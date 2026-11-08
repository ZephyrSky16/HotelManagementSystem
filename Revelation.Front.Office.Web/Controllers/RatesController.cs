using System;
using System.Linq;
using System.Web.Mvc;
using Revelation.Front.Office.Core.Interfaces.Services;
using Revelation.Front.Office.Models.Entities;
using Revelation.Front.Office.Services.Implementations;
using Revelation.Front.Office.Web.ViewModels;

namespace Revelation.Front.Office.Web.Controllers
{
    public class RatesController : Controller
    {
        private readonly IRateService _rateService;
        private readonly IRoomTypeService _roomTypeService;

        public RatesController()
        {
            _rateService = new RateService();
            _roomTypeService = new RoomTypeService();
        }

        public ActionResult Index()
        {
            var vm = new RateListViewModel
            {
                RatePlans = _rateService.GetAll().ToList(),
                RoomTypes = _roomTypeService.GetAll().ToList(),
                ExtraCharges = Revelation.Front.Office.Services.MockData.MockDataStore.ExtraCharges
            };
            return View(vm);
        }

        [HttpGet]
        public JsonResult GetRateForDate(int roomTypeId, string date)
        {
            var d = DateTime.Parse(date);
            var plan = _rateService.ResolveRateForDate(roomTypeId, d);
            if (plan == null)
            {
                var type = _roomTypeService.GetById(roomTypeId);
                return Json(new { planId = (int?)null, planName = "Base Rate (no plan configured)", rate = type.BaseRate }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { planId = plan.Id, planName = plan.RatePlanName, planType = plan.RatePlanType.ToString(), rate = plan.DailyRate }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveRatePlan(int? id, int roomTypeId, string ratePlanName, RatePlanType ratePlanType, DateTime effectiveStart, DateTime effectiveEnd, decimal dailyRate, int minStay, int maxStay, bool active)
        {
            var type = _roomTypeService.GetById(roomTypeId);
            if (type == null || string.IsNullOrWhiteSpace(ratePlanName) || dailyRate <= 0)
            {
                return Json(new { success = false, message = "Please complete all required fields." });
            }

            if (id.HasValue)
            {
                var plan = _rateService.GetById(id.Value);
                if (plan == null) { return Json(new { success = false, message = "Rate plan not found." }); }
                plan.RoomTypeId = roomTypeId; plan.RoomType = type; plan.RatePlanName = ratePlanName; plan.RatePlanType = ratePlanType;
                plan.EffectiveStart = effectiveStart; plan.EffectiveEnd = effectiveEnd; plan.DailyRate = dailyRate;
                plan.MinStay = minStay; plan.MaxStay = maxStay; plan.Active = active;
                _rateService.Update(plan);
            }
            else
            {
                _rateService.Create(new RatePlan
                {
                    RoomTypeId = roomTypeId, RoomType = type, RatePlanName = ratePlanName, RatePlanType = ratePlanType,
                    EffectiveStart = effectiveStart, EffectiveEnd = effectiveEnd, DailyRate = dailyRate, MinStay = minStay, MaxStay = maxStay, Active = active
                });
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult ToggleActive(int id)
        {
            var plan = _rateService.GetById(id);
            if (plan == null) { return Json(new { success = false }); }
            _rateService.SetActive(id, !plan.Active);
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult SaveExtraCharge(int roomTypeId, decimal extraPersonCharge, decimal childRate)
        {
            _rateService.SaveExtraCharge(new ExtraCharge { RoomTypeId = roomTypeId, ExtraPersonCharge = extraPersonCharge, ChildRate = childRate });
            return Json(new { success = true });
        }
    }
}
