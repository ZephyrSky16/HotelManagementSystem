using System;
using System.Collections.Generic;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Services.MockData
{
    public static partial class MockDataStore
    {
        private static List<RatePlan> SeedRatePlans(List<RoomType> types)
        {
            var std = RoomTypeByName(types, "Standard Room");
            var dlx = RoomTypeByName(types, "Deluxe Room");
            var sup = RoomTypeByName(types, "Superior Room");
            var fam = RoomTypeByName(types, "Family Room");
            var ste = RoomTypeByName(types, "Suite");
            var exe = RoomTypeByName(types, "Executive Suite");

            RatePlan P(RoomType type, string name, RatePlanType planType, DateTime start, DateTime end, decimal rate, int minStay, int maxStay, bool active) =>
                new RatePlan
                {
                    Id = NextId(), RoomType = type, RoomTypeId = type.Id, RatePlanName = name, RatePlanType = planType,
                    EffectiveStart = start, EffectiveEnd = end, DailyRate = rate, MinStay = minStay, MaxStay = maxStay, Active = active
                };

            var yearStart = new DateTime(2026, 1, 1);
            var yearEnd = new DateTime(2026, 12, 31);

            return new List<RatePlan>
            {
                P(std, "Standard Rate", RatePlanType.Standard, yearStart, yearEnd, 89.00m, 1, 30, true),
                P(dlx, "Standard Rate", RatePlanType.Standard, yearStart, yearEnd, 119.00m, 1, 30, true),
                P(sup, "Standard Rate", RatePlanType.Standard, yearStart, yearEnd, 139.00m, 1, 30, true),
                P(fam, "Standard Rate", RatePlanType.Standard, yearStart, yearEnd, 159.00m, 1, 30, true),
                P(ste, "Standard Rate", RatePlanType.Standard, yearStart, yearEnd, 219.00m, 1, 45, true),
                P(exe, "Standard Rate", RatePlanType.Standard, yearStart, yearEnd, 299.00m, 1, 45, true),

                P(std, "Weekend Rate", RatePlanType.Weekend, yearStart, yearEnd, 102.00m, 1, 4, true),
                P(dlx, "Weekend Rate", RatePlanType.Weekend, yearStart, yearEnd, 137.00m, 1, 4, true),
                P(sup, "Weekend Rate", RatePlanType.Weekend, yearStart, yearEnd, 160.00m, 1, 4, true),
                P(fam, "Weekend Rate", RatePlanType.Weekend, yearStart, yearEnd, 183.00m, 1, 4, true),
                P(ste, "Weekend Rate", RatePlanType.Weekend, yearStart, yearEnd, 252.00m, 1, 4, true),
                P(exe, "Weekend Rate", RatePlanType.Weekend, yearStart, yearEnd, 344.00m, 1, 4, true),

                P(ste, "Summer Seasonal Rate", RatePlanType.Seasonal, new DateTime(2027, 6, 1), new DateTime(2027, 8, 31), 259.00m, 2, 14, true),
                P(exe, "Summer Seasonal Rate", RatePlanType.Seasonal, new DateTime(2027, 6, 1), new DateTime(2027, 8, 31), 349.00m, 2, 14, true),
                P(fam, "Summer Seasonal Rate", RatePlanType.Seasonal, new DateTime(2027, 6, 1), new DateTime(2027, 8, 31), 189.00m, 2, 14, true),

                P(ste, "Year-End Holiday Rate", RatePlanType.Holiday, new DateTime(2026, 12, 20), new DateTime(2027, 1, 3), 299.00m, 3, 10, true),
                P(exe, "Year-End Holiday Rate", RatePlanType.Holiday, new DateTime(2026, 12, 20), new DateTime(2027, 1, 3), 399.00m, 3, 10, true),

                P(std, "Early Bird Promo", RatePlanType.Promotional, new DateTime(2026, 11, 1), new DateTime(2026, 11, 30), 75.00m, 3, 7, true),
                P(dlx, "Early Bird Promo", RatePlanType.Promotional, new DateTime(2026, 11, 1), new DateTime(2026, 11, 30), 99.00m, 3, 7, true),
                P(sup, "Stay Longer Save More", RatePlanType.Promotional, new DateTime(2026, 11, 15), new DateTime(2027, 2, 28), 119.00m, 5, 21, false)
            };
        }

        private static List<ExtraCharge> SeedExtraCharges(List<RoomType> types)
        {
            ExtraCharge E(string typeName, decimal extraPerson, decimal child)
            {
                var t = RoomTypeByName(types, typeName);
                return new ExtraCharge { Id = NextId(), RoomType = t, RoomTypeId = t.Id, ExtraPersonCharge = extraPerson, ChildRate = child };
            }

            return new List<ExtraCharge>
            {
                E("Standard Room", 18.00m, 9.00m),
                E("Deluxe Room", 22.00m, 11.00m),
                E("Superior Room", 25.00m, 12.00m),
                E("Family Room", 20.00m, 10.00m),
                E("Suite", 35.00m, 15.00m),
                E("Executive Suite", 45.00m, 20.00m)
            };
        }
    }
}
