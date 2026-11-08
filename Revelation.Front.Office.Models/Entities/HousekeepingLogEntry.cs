using System;
using Revelation.Front.Office.Models.Common;

namespace Revelation.Front.Office.Models.Entities
{
    /// <summary>A single housekeeping status transition, used to build the cleaning history view.</summary>
    public class HousekeepingLogEntry : BaseEntity
    {
        public int RoomId { get; set; }
        public virtual Room Room { get; set; }

        public string Staff { get; set; }
        public DateTime EntryDate { get; set; } = DateTime.Now;
        public HousekeepingStatus FromStatus { get; set; }
        public HousekeepingStatus ToStatus { get; set; }
        public int DurationMinutes { get; set; }
    }
}
