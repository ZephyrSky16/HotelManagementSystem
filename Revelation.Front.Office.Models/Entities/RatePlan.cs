using System;
using Revelation.Front.Office.Models.Common;

namespace Revelation.Front.Office.Models.Entities
{
    public enum RatePlanType
    {
        Standard = 0,
        Weekday = 1,
        Weekend = 2,
        Seasonal = 3,
        Holiday = 4,
        Promotional = 5
    }

    public class RatePlan : BaseEntity
    {
        public int RoomTypeId { get; set; }
        public virtual RoomType RoomType { get; set; }

        public string RatePlanName { get; set; }
        public RatePlanType RatePlanType { get; set; }
        public DateTime EffectiveStart { get; set; }
        public DateTime EffectiveEnd { get; set; }
        public decimal DailyRate { get; set; }
        public int MinStay { get; set; } = 1;
        public int MaxStay { get; set; } = 30;
        public bool Active { get; set; } = true;
    }
}
