using System;
using System.Collections.Generic;
using System.Linq;
using Revelation.Front.Office.Core.Interfaces.Services;
using Revelation.Front.Office.Models.Entities;
using Revelation.Front.Office.Services.MockData;

namespace Revelation.Front.Office.Services.Implementations
{
    public class RateService : IRateService
    {
        private static readonly RatePlanType[] Priority =
        {
            RatePlanType.Holiday, RatePlanType.Seasonal, RatePlanType.Promotional, RatePlanType.Weekend, RatePlanType.Weekday, RatePlanType.Standard
        };

        public IEnumerable<RatePlan> GetAll() => MockDataStore.RatePlans.OrderBy(p => p.RoomType.Name).ThenBy(p => p.RatePlanType);

        public RatePlan GetById(int id) => MockDataStore.RatePlans.FirstOrDefault(p => p.Id == id);

        public RatePlan Create(RatePlan plan)
        {
            plan.Id = MockDataStore.NextId();
            MockDataStore.RatePlans.Add(plan);
            return plan;
        }

        public void Update(RatePlan plan)
        {
            var existing = GetById(plan.Id);
            if (existing == null) return;

            existing.RoomTypeId = plan.RoomTypeId;
            existing.RoomType = plan.RoomType;
            existing.RatePlanName = plan.RatePlanName;
            existing.RatePlanType = plan.RatePlanType;
            existing.EffectiveStart = plan.EffectiveStart;
            existing.EffectiveEnd = plan.EffectiveEnd;
            existing.DailyRate = plan.DailyRate;
            existing.MinStay = plan.MinStay;
            existing.MaxStay = plan.MaxStay;
            existing.ModifiedDate = DateTime.Now;
        }

        public void SetActive(int id, bool active)
        {
            var plan = GetById(id);
            if (plan == null) return;
            plan.Active = active;
            plan.ModifiedDate = DateTime.Now;
        }

        public RatePlan ResolveRateForDate(int roomTypeId, DateTime date)
        {
            var dow = date.DayOfWeek;
            var candidates = MockDataStore.RatePlans.Where(p =>
                p.RoomTypeId == roomTypeId && p.Active &&
                date.Date >= p.EffectiveStart.Date && date.Date <= p.EffectiveEnd.Date &&
                (p.RatePlanType != RatePlanType.Weekend || dow == DayOfWeek.Friday || dow == DayOfWeek.Saturday || dow == DayOfWeek.Sunday) &&
                (p.RatePlanType != RatePlanType.Weekday || (dow != DayOfWeek.Friday && dow != DayOfWeek.Saturday && dow != DayOfWeek.Sunday)))
                .ToList();

            return candidates.OrderBy(p => Array.IndexOf(Priority, p.RatePlanType)).FirstOrDefault();
        }

        public ExtraCharge GetExtraCharge(int roomTypeId) =>
            MockDataStore.ExtraCharges.FirstOrDefault(e => e.RoomTypeId == roomTypeId);

        public void SaveExtraCharge(ExtraCharge extraCharge)
        {
            var existing = GetExtraCharge(extraCharge.RoomTypeId);
            if (existing == null)
            {
                extraCharge.Id = MockDataStore.NextId();
                MockDataStore.ExtraCharges.Add(extraCharge);
            }
            else
            {
                existing.ExtraPersonCharge = extraCharge.ExtraPersonCharge;
                existing.ChildRate = extraCharge.ChildRate;
            }
        }
    }
}
