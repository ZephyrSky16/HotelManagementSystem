using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using log4net;
using Revelation.Front.Office.Core.Interfaces.HotellingSystem;
using Revelation.Front.Office.Repositories.HotellingSystem;
using Revelation.Front.Office.Services.HotellingSystem;
using Revelation.Front.Office.Web.HotellingSystem;
using Revelation.Front.Office.Web.ViewModels;
using HS = Revelation.Front.Office.Models.HotellingSystem;

namespace Revelation.Front.Office.Web.Controllers
{
    /// <summary>CRUD over dbo.Reservation in the HotellingSystem database.</summary>
    public class ReservationRecordsController : HotellingSystemController
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ReservationRecordsController));

        private IHsReservationService _reservations;
        private IHsReservationService Reservations =>
            _reservations ?? (_reservations = new HsReservationService(new HsReservationRepository(Db)));

        public ActionResult Index()
        {
            return View(Reservations.GetAll().ToList());
        }

        public ActionResult Create()
        {
            return View("Form", WithLookups(new ReservationRecordFormViewModel()));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ReservationRecordFormViewModel model)
        {
            if (!ModelState.IsValid) { return View("Form", WithLookups(model)); }

            var reservation = new HS.Reservation();
            model.ApplyTo(reservation);
            if (!TrySave(() => Reservations.Create(reservation))) { return View("Form", WithLookups(model)); }

            TempData["Success"] = "Reservation #" + reservation.Id + " created.";
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var reservation = Reservations.GetById(id);
            if (reservation == null) { return HttpNotFound(); }
            return View("Form", WithLookups(ReservationRecordFormViewModel.FromEntity(reservation)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ReservationRecordFormViewModel model)
        {
            model.Id = id;
            if (!ModelState.IsValid) { return View("Form", WithLookups(model)); }

            var reservation = Reservations.GetById(id);
            if (reservation == null) { return HttpNotFound(); }
            model.ApplyTo(reservation);
            if (!TrySave(() => Reservations.Update(reservation))) { return View("Form", WithLookups(model)); }

            TempData["Success"] = "Reservation #" + id + " updated.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            try
            {
                Reservations.Delete(id);
                return Json(new { success = true });
            }
            catch (KeyNotFoundException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                Log.Error("Deleting HotellingSystem reservation " + id + " failed", ex);
                return Json(new { success = false, message = "The reservation could not be deleted. See the application log for details." });
            }
        }

        private bool TrySave(Action save)
        {
            try
            {
                save();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Saving HotellingSystem reservation failed", ex);
                ModelState.AddModelError("", "The reservation could not be saved: " + ex.GetBaseException().Message);
                return false;
            }
        }

        private ReservationRecordFormViewModel WithLookups(ReservationRecordFormViewModel m)
        {
            m.ReservationStatuses = Options<HS.ReservationStatus>(m.ReservationStatusId);
            m.BookingCodes = Options<HS.BookingCode>(m.BookingCodeId);
            m.MarketCodes = Options<HS.MarketCode>(m.MarketCodeId);
            m.Rates = Options<HS.Rate>(m.RateId);
            m.Agents = Options<HS.Agent>(m.AgentId);
            m.Sales = Options<HS.Sale>(m.SalesId);
            m.ConfirmTos = Options<HS.ConfirmTo>(m.ConfirmToId);
            m.ConfirmVias = Options<HS.ConfirmVia>(m.ConfirmViaId);
            m.Cancellations = Options<HS.Cancellation>(m.LastCancellationId);
            return m;
        }

        private IEnumerable<SelectListItem> Options<T>(int? selectedId) where T : HS.HsEntity =>
            ReservationRecordFormViewModel.Options(MasterDataRegistry.CreateService<T>(Db).GetAll(), selectedId);
    }
}
