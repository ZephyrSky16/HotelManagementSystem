using System;
using System.Linq;
using System.Web.Mvc;
using Revelation.Front.Office.Core.Interfaces.Services;
using Revelation.Front.Office.Models.Common;
using Revelation.Front.Office.Services.Implementations;
using Revelation.Front.Office.Web.ViewModels;

namespace Revelation.Front.Office.Web.Controllers
{
    public class CheckOutController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IReservationService _reservationService;

        public CheckOutController()
        {
            _roomService = new RoomService();
            _reservationService = new ReservationService(_roomService);
        }

        public ActionResult Index(string res)
        {
            var vm = new CheckOutIndexViewModel
            {
                InHouseGuests = _reservationService.GetInHouse().ToList()
            };
            return View(vm);
        }

        [HttpGet]
        public JsonResult GetFolio(int id)
        {
            var r = _reservationService.GetById(id);
            if (r == null) { return Json(new { found = false }, JsonRequestBehavior.AllowGet); }

            return Json(new
            {
                found = true, id = r.Id, reservationNo = r.ReservationNo, guestName = r.Guest.FullName,
                roomNumber = r.Room.Number, roomType = r.RoomType.Name,
                checkIn = r.CheckIn.ToString("MMM d, yyyy"), checkOut = r.CheckOut.ToString("MMM d, yyyy"), nights = r.Nights,
                rate = r.Rate, subtotal = r.Subtotal, extraCharges = r.ExtraCharges, taxesAndFees = r.TaxesAndFees, discount = r.Discount,
                totalAmount = r.TotalAmount, amountPaid = r.AmountPaid, balance = r.Balance
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddCharge(int reservationId, decimal amount)
        {
            var r = _reservationService.GetById(reservationId);
            if (r == null || amount <= 0) { return Json(new { success = false }); }

            r.ExtraCharges += amount;
            r.TotalAmount += amount;
            r.Balance = Math.Max(0, r.TotalAmount - r.AmountPaid);
            return Json(new { success = true, totalAmount = r.TotalAmount, balance = r.Balance });
        }

        [HttpPost]
        public JsonResult RecordPayment(int reservationId, decimal amount)
        {
            _reservationService.RecordPayment(reservationId, amount);
            var r = _reservationService.GetById(reservationId);
            return Json(new { success = true, amountPaid = r.AmountPaid, balance = r.Balance });
        }

        [HttpPost]
        public JsonResult Confirm(int reservationId)
        {
            var r = _reservationService.GetById(reservationId);
            if (r == null) { return Json(new { success = false, message = "Reservation not found." }); }
            if (r.Balance > 0) { return Json(new { success = false, message = "Outstanding balance of " + r.Balance.ToString("C") + " must be settled before check-out." }); }

            var roomNumber = r.Room.Number;
            _reservationService.CheckOut(reservationId);
            return Json(new { success = true, roomNumber = roomNumber, guestName = r.Guest.FullName });
        }
    }
}
