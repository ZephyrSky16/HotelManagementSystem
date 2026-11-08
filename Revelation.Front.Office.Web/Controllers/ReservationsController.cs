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
    public class ReservationsController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IReservationService _reservationService;
        private readonly IRoomTypeService _roomTypeService;
        private readonly IGuestService _guestService;
        private readonly IRateService _rateService;

        public ReservationsController()
        {
            _roomService = new RoomService();
            _reservationService = new ReservationService(_roomService);
            _roomTypeService = new RoomTypeService();
            _guestService = new GuestService();
            _rateService = new RateService();
        }

        public ActionResult Index(string res)
        {
            var vm = new ReservationListViewModel
            {
                Reservations = _reservationService.GetAll().ToList(),
                RoomTypes = _roomTypeService.GetAll().ToList()
            };
            ViewBag.HighlightReservationNo = res;
            return View(vm);
        }

        public ActionResult InHouse()
        {
            var vm = new ReservationListViewModel { Reservations = _reservationService.GetInHouse().ToList() };
            return View(vm);
        }

        public ActionResult Create()
        {
            var vm = new ReservationFormViewModel
            {
                RoomTypes = _roomTypeService.GetAll().ToList(),
                Guests = _guestService.GetAll().ToList()
            };
            return View("Form", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ReservationFormViewModel vm)
        {
            if (vm.CheckOut <= vm.CheckIn) { ModelState.AddModelError("CheckOut", "Check-out must be after check-in."); }
            if (!vm.SelectedRoomId.HasValue) { ModelState.AddModelError("SelectedRoomId", "Please search and select an available room."); }

            if (!ModelState.IsValid)
            {
                vm.RoomTypes = _roomTypeService.GetAll().ToList();
                vm.Guests = _guestService.GetAll().ToList();
                return View("Form", vm);
            }

            var room = _roomService.GetById(vm.SelectedRoomId.Value);
            var guest = _guestService.Create(new Guest
            {
                FirstName = vm.FirstName, LastName = vm.LastName, Email = vm.Email, Phone = vm.Phone,
                Address = vm.Address, Nationality = vm.Nationality, IdType = vm.IdType, IdNumber = vm.IdNumber
            });

            var nights = Math.Max(1, (vm.CheckOut.Date - vm.CheckIn.Date).Days);
            var rate = room.BaseRate;
            var subtotal = rate * nights;
            var tax = Math.Round(subtotal * 0.12m, 2);
            var total = Math.Round(subtotal + tax - vm.Discount, 2);

            var reservation = new Reservation
            {
                GuestId = guest.Id, Guest = guest, RoomTypeId = room.RoomTypeId, RoomType = room.RoomType, RoomId = room.Id,
                CheckIn = vm.CheckIn, CheckOut = vm.CheckOut, Adults = vm.Adults, Children = vm.Children,
                Rate = rate, Subtotal = subtotal, TaxesAndFees = tax, Discount = vm.Discount, TotalAmount = total,
                AmountPaid = vm.AmountPaid, Balance = Math.Max(0, total - vm.AmountPaid),
                PaymentMethod = vm.PaymentMethod, PaymentStatus = vm.AmountPaid <= 0 ? PaymentStatus.Unpaid : (vm.AmountPaid >= total ? PaymentStatus.Paid : PaymentStatus.PartiallyPaid),
                PaymentReference = vm.PaymentReference, BookingStatus = BookingStatus.Confirmed, BookingSource = vm.BookingSource,
                SpecialRequests = vm.SpecialRequests, StaffNotes = vm.StaffNotes
            };
            _reservationService.Create(reservation);

            TempData["Success"] = "Reservation " + reservation.ReservationNo + " created for " + guest.FullName + ".";
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var reservation = _reservationService.GetById(id);
            if (reservation == null) { return HttpNotFound(); }

            var vm = new ReservationFormViewModel
            {
                Id = reservation.Id, ReservationNo = reservation.ReservationNo,
                FirstName = reservation.Guest.FirstName, LastName = reservation.Guest.LastName, Email = reservation.Guest.Email,
                Phone = reservation.Guest.Phone, Address = reservation.Guest.Address, Nationality = reservation.Guest.Nationality,
                IdType = reservation.Guest.IdType, IdNumber = reservation.Guest.IdNumber,
                CheckIn = reservation.CheckIn, CheckOut = reservation.CheckOut, Adults = reservation.Adults, Children = reservation.Children,
                RoomTypeId = reservation.RoomTypeId, SelectedRoomId = reservation.RoomId, Rate = reservation.Rate, Discount = reservation.Discount,
                AmountPaid = reservation.AmountPaid, PaymentMethod = reservation.PaymentMethod, PaymentStatus = reservation.PaymentStatus,
                PaymentReference = reservation.PaymentReference, SpecialRequests = reservation.SpecialRequests, StaffNotes = reservation.StaffNotes,
                BookingSource = reservation.BookingSource,
                RoomTypes = _roomTypeService.GetAll().ToList(), Guests = _guestService.GetAll().ToList()
            };
            return View("Form", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ReservationFormViewModel vm)
        {
            var reservation = _reservationService.GetById(id);
            if (reservation == null) { return HttpNotFound(); }

            if (vm.CheckOut <= vm.CheckIn) { ModelState.AddModelError("CheckOut", "Check-out must be after check-in."); }
            if (!vm.SelectedRoomId.HasValue) { ModelState.AddModelError("SelectedRoomId", "Please select a room."); }

            if (!ModelState.IsValid)
            {
                vm.Id = id;
                vm.RoomTypes = _roomTypeService.GetAll().ToList();
                vm.Guests = _guestService.GetAll().ToList();
                return View("Form", vm);
            }

            var room = _roomService.GetById(vm.SelectedRoomId.Value);
            var nights = Math.Max(1, (vm.CheckOut.Date - vm.CheckIn.Date).Days);
            var subtotal = room.BaseRate * nights;
            var tax = Math.Round(subtotal * 0.12m, 2);
            var total = Math.Round(subtotal + tax - vm.Discount, 2);

            reservation.Guest.FirstName = vm.FirstName; reservation.Guest.LastName = vm.LastName; reservation.Guest.Email = vm.Email;
            reservation.Guest.Phone = vm.Phone; reservation.Guest.Address = vm.Address; reservation.Guest.Nationality = vm.Nationality;
            reservation.Guest.IdType = vm.IdType; reservation.Guest.IdNumber = vm.IdNumber;

            reservation.RoomTypeId = room.RoomTypeId; reservation.RoomType = room.RoomType; reservation.RoomId = room.Id;
            reservation.CheckIn = vm.CheckIn; reservation.CheckOut = vm.CheckOut; reservation.Adults = vm.Adults; reservation.Children = vm.Children;
            reservation.Rate = room.BaseRate; reservation.Subtotal = subtotal; reservation.TaxesAndFees = tax; reservation.Discount = vm.Discount;
            reservation.TotalAmount = total; reservation.AmountPaid = vm.AmountPaid; reservation.Balance = Math.Max(0, total - vm.AmountPaid);
            reservation.PaymentMethod = vm.PaymentMethod; reservation.PaymentReference = vm.PaymentReference;
            reservation.PaymentStatus = vm.AmountPaid <= 0 ? PaymentStatus.Unpaid : (vm.AmountPaid >= total ? PaymentStatus.Paid : PaymentStatus.PartiallyPaid);
            reservation.SpecialRequests = vm.SpecialRequests; reservation.StaffNotes = vm.StaffNotes; reservation.BookingSource = vm.BookingSource;

            _reservationService.Update(reservation);

            TempData["Success"] = "Reservation " + reservation.ReservationNo + " updated.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult GetAvailableRooms(int roomTypeId, string checkIn, string checkOut, int? excludeReservationId)
        {
            var ci = DateTime.Parse(checkIn);
            var co = DateTime.Parse(checkOut);
            var rooms = _reservationService.FindAvailableRooms(roomTypeId, ci, co, excludeReservationId)
                .Select(r => new { id = r.Id, number = r.Number, floor = r.Floor, bed = r.BedConfiguration, rate = r.BaseRate })
                .ToList();
            return Json(rooms, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult QuickCheckIn(int reservationId)
        {
            var reservation = _reservationService.GetById(reservationId);
            if (reservation?.RoomId == null) { return Json(new { success = false, message = "Assign a room before checking in." }); }
            _reservationService.CheckIn(reservationId);
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult CancelReservation(int reservationId)
        {
            _reservationService.Cancel(reservationId);
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult ChangeRoom(int reservationId, int roomId)
        {
            _reservationService.AssignRoom(reservationId, roomId);
            return Json(new { success = true });
        }

        [HttpGet]
        public JsonResult GetChangeRoomOptions(int reservationId)
        {
            var reservation = _reservationService.GetById(reservationId);
            if (reservation == null) { return Json(new { rooms = new object[0] }, JsonRequestBehavior.AllowGet); }

            var rooms = _reservationService.FindAvailableRooms(reservation.RoomTypeId, reservation.CheckIn, reservation.CheckOut, reservation.Id)
                .Where(r => r.Id != reservation.RoomId)
                .Select(r => new { id = r.Id, number = r.Number, floor = r.Floor, rate = r.BaseRate })
                .ToList();
            return Json(new { rooms = rooms }, JsonRequestBehavior.AllowGet);
        }
    }
}
