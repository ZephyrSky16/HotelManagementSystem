using System;
using System.Linq;
using System.Web.Mvc;
using Revelation.Front.Office.Core.Interfaces.Services;
using Revelation.Front.Office.Models.Common;
using Revelation.Front.Office.Services.Implementations;
using Revelation.Front.Office.Web.ViewModels;

namespace Revelation.Front.Office.Web.Controllers
{
    public class CheckInController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IReservationService _reservationService;

        public CheckInController()
        {
            _roomService = new RoomService();
            _reservationService = new ReservationService(_roomService);
        }

        public ActionResult Index(string res)
        {
            var today = DateTime.Today;
            var vm = new CheckInIndexViewModel
            {
                PendingArrivals = _reservationService.GetAll()
                    .Where(r => (r.BookingStatus == BookingStatus.Confirmed || r.BookingStatus == BookingStatus.Tentative) && r.CheckIn.Date <= today)
                    .OrderBy(r => r.CheckIn).ToList()
            };
            ViewBag.PreselectReservationNo = res;
            return View(vm);
        }

        [HttpGet]
        public JsonResult GetReservation(int id)
        {
            var r = _reservationService.GetById(id);
            if (r == null) { return Json(new { found = false }, JsonRequestBehavior.AllowGet); }

            return Json(new
            {
                found = true, id = r.Id, reservationNo = r.ReservationNo, guestName = r.Guest.FullName,
                phone = r.Guest.Phone, email = r.Guest.Email, nationality = r.Guest.Nationality,
                idType = r.Guest.IdType.ToString(), idNumber = r.Guest.IdNumber,
                roomId = r.RoomId, roomNumber = r.Room != null ? r.Room.Number : null, roomType = r.RoomType.Name,
                checkIn = r.CheckIn.ToString("MMM d, yyyy"), checkOut = r.CheckOut.ToString("MMM d, yyyy"),
                nights = r.Nights, adults = r.Adults, children = r.Children,
                totalAmount = r.TotalAmount, amountPaid = r.AmountPaid, balance = r.Balance,
                bookingStatus = r.BookingStatus.ToString()
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAlternateRooms(int reservationId)
        {
            var r = _reservationService.GetById(reservationId);
            if (r == null) { return Json(new { rooms = new object[0] }, JsonRequestBehavior.AllowGet); }
            var rooms = _reservationService.FindAvailableRooms(r.RoomTypeId, r.CheckIn, r.CheckOut, r.Id)
                .Select(room => new { id = room.Id, number = room.Number, floor = room.Floor }).ToList();
            return Json(new { rooms = rooms }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AssignRoom(int reservationId, int roomId)
        {
            _reservationService.AssignRoom(reservationId, roomId);
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult RecordPayment(int reservationId, decimal amount)
        {
            _reservationService.RecordPayment(reservationId, amount);
            var r = _reservationService.GetById(reservationId);
            return Json(new { success = true, amountPaid = r.AmountPaid, balance = r.Balance });
        }

        [HttpPost]
        public JsonResult Confirm(int reservationId, string phone, string email, string nationality, string idNumber)
        {
            var r = _reservationService.GetById(reservationId);
            if (r == null) { return Json(new { success = false, message = "Reservation not found." }); }
            if (!r.RoomId.HasValue) { return Json(new { success = false, message = "Assign a room before confirming check-in." }); }

            r.Guest.Phone = phone; r.Guest.Email = email; r.Guest.Nationality = nationality; r.Guest.IdNumber = idNumber;
            _reservationService.CheckIn(reservationId);
            return Json(new { success = true, roomNumber = r.Room.Number, guestName = r.Guest.FullName });
        }
    }
}
