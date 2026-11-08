using System;
using System.Linq;
using System.Web.Mvc;
using Revelation.Front.Office.Core.Interfaces.Services;
using Revelation.Front.Office.Models.Common;
using Revelation.Front.Office.Models.Entities;
using Revelation.Front.Office.Services.Implementations;
using Revelation.Front.Office.Web.ViewModels;

namespace Revelation.Front.Office.Web.Controllers
{
    public class CalendarController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IReservationService _reservationService;
        private readonly IRoomTypeService _roomTypeService;
        private readonly IGuestService _guestService;

        public CalendarController()
        {
            _roomService = new RoomService();
            _reservationService = new ReservationService(_roomService);
            _roomTypeService = new RoomTypeService();
            _guestService = new GuestService();
        }

        public ActionResult Index()
        {
            var vm = new CalendarIndexViewModel
            {
                RoomTypes = _roomTypeService.GetAll().ToList(),
                Floors = _roomService.GetAll().Select(r => r.Floor).Distinct().OrderBy(f => f).ToList()
            };
            return View(vm);
        }

        [HttpGet]
        public JsonResult GetRackData(string start, string end, int? roomTypeId, int? floor, string roomStatus, string search)
        {
            var rooms = _roomService.GetAll().AsEnumerable();
            if (roomTypeId.HasValue) { rooms = rooms.Where(r => r.RoomTypeId == roomTypeId.Value); }
            if (floor.HasValue) { rooms = rooms.Where(r => r.Floor == floor.Value); }
            if (!string.IsNullOrEmpty(roomStatus))
            {
                rooms = rooms.Where(r => r.Status.ToString() == roomStatus || r.HousekeepingStatus.ToString() == roomStatus);
            }

            var reservations = _reservationService.GetAll().ToList();

            if (!string.IsNullOrEmpty(search))
            {
                var q = search.ToLowerInvariant();
                var matchingRoomIds = reservations
                    .Where(r => r.Guest.FullName.ToLowerInvariant().Contains(q) || r.ReservationNo.ToLowerInvariant().Contains(q))
                    .Select(r => r.RoomId).Where(id => id.HasValue).Select(id => id.Value).ToList();
                rooms = rooms.Where(r => matchingRoomIds.Contains(r.Id));
            }

            var roomList = rooms.ToList();
            var roomIds = roomList.Select(r => r.Id).ToList();

            var roomDtos = roomList.OrderBy(r => r.Number).Select(r => new RackRoomDto
            {
                Id = r.Id, Number = r.Number, Type = r.RoomType.Name, Floor = r.Floor,
                Status = r.Status.ToString(), HousekeepingStatus = r.HousekeepingStatus.ToString()
            }).ToList();

            var reservationDtos = reservations
                .Where(r => r.RoomId.HasValue && roomIds.Contains(r.RoomId.Value) && r.BookingStatus != BookingStatus.Cancelled)
                .Select(r => new RackReservationDto
                {
                    Id = r.Id, ReservationNo = r.ReservationNo, GuestName = r.Guest.FullName, ContactNumber = r.Guest.Phone,
                    RoomId = r.RoomId, RoomType = r.RoomType.Name, RoomTypeId = r.RoomTypeId,
                    CheckIn = r.CheckIn.ToString("yyyy-MM-dd"), CheckOut = r.CheckOut.ToString("yyyy-MM-dd"),
                    BookingStatus = r.BookingStatus.ToString(), PaymentStatus = r.PaymentStatus.ToString(),
                    TotalAmount = r.TotalAmount, AmountPaid = r.AmountPaid, Balance = r.Balance,
                    Nights = r.Nights, Adults = r.Adults, Children = r.Children
                }).ToList();

            return Json(new { rooms = roomDtos, reservations = reservationDtos }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreateBooking(int roomId, string checkIn, string checkOut, string guestName, string phone, int adults, int children, string source)
        {
            var room = _roomService.GetById(roomId);
            if (room == null) { return Json(new { success = false, message = "Room not found." }); }

            var ci = DateTime.Parse(checkIn);
            var co = DateTime.Parse(checkOut);
            if (co <= ci) { return Json(new { success = false, message = "Check-out must be after check-in." }); }

            var available = _reservationService.FindAvailableRooms(room.RoomTypeId, ci, co).Any(r => r.Id == roomId);
            if (!available) { return Json(new { success = false, message = "Room " + room.Number + " is already booked for part of that date range." }); }

            var nameParts = (guestName ?? "").Trim().Split(new[] { ' ' }, 2);
            var guest = _guestService.Create(new Guest
            {
                FirstName = nameParts.Length > 0 ? nameParts[0] : "Guest",
                LastName = nameParts.Length > 1 ? nameParts[1] : "",
                Phone = phone
            });

            var nights = Math.Max(1, (co - ci).Days);
            var subtotal = room.BaseRate * nights;
            var tax = Math.Round(subtotal * 0.12m, 2);
            var total = Math.Round(subtotal + tax, 2);

            BookingSource bookingSource;
            Enum.TryParse(source, out bookingSource);

            var reservation = new Reservation
            {
                GuestId = guest.Id, Guest = guest, RoomTypeId = room.RoomTypeId, RoomType = room.RoomType, RoomId = room.Id,
                CheckIn = ci, CheckOut = co, Adults = adults > 0 ? adults : 1, Children = children,
                Rate = room.BaseRate, Subtotal = subtotal, TaxesAndFees = tax, TotalAmount = total,
                AmountPaid = 0, Balance = total, BookingStatus = BookingStatus.Confirmed, BookingSource = bookingSource,
                StaffNotes = "Created from the Room Availability Calendar."
            };
            _reservationService.Create(reservation);

            return Json(new { success = true, reservationNo = reservation.ReservationNo });
        }

        [HttpPost]
        public JsonResult MoveReservation(int reservationId, int targetRoomId)
        {
            var reservation = _reservationService.GetById(reservationId);
            var targetRoom = _roomService.GetById(targetRoomId);
            if (reservation == null || targetRoom == null) { return Json(new { success = false, message = "Reservation or room not found." }); }

            if (targetRoom.Status == RoomStatus.OutOfOrder || targetRoom.Status == RoomStatus.UnderMaintenance)
            {
                return Json(new { success = false, message = "Room " + targetRoom.Number + " is not available for a room change." });
            }

            var overlap = _reservationService.GetAll().Any(r =>
                r.RoomId == targetRoomId && r.Id != reservationId &&
                r.BookingStatus != BookingStatus.Cancelled && r.BookingStatus != BookingStatus.NoShow &&
                reservation.CheckOut > r.CheckIn && reservation.CheckIn < r.CheckOut);
            if (overlap) { return Json(new { success = false, message = "Room " + targetRoom.Number + " already has a booking in that date range." }); }

            var oldRoomNumber = reservation.Room != null ? reservation.Room.Number : "";
            _reservationService.AssignRoom(reservationId, targetRoomId);
            return Json(new { success = true, message = "Moved " + reservation.Guest.FullName + " from Room " + oldRoomNumber + " to Room " + targetRoom.Number + "." });
        }

        [HttpPost]
        public JsonResult CheckIn(int reservationId)
        {
            _reservationService.CheckIn(reservationId);
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult CancelReservation(int reservationId)
        {
            _reservationService.Cancel(reservationId);
            return Json(new { success = true });
        }
    }
}
