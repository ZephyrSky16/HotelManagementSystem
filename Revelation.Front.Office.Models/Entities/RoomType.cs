using System.Collections.Generic;
using Revelation.Front.Office.Models.Common;

namespace Revelation.Front.Office.Models.Entities
{
    public class RoomType : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int MaxOccupancy { get; set; }
        public int MaxAdults { get; set; }
        public int MaxChildren { get; set; }
        public string BedConfiguration { get; set; }
        public decimal BaseRate { get; set; }
        public bool Active { get; set; } = true;

        /// <summary>Comma-separated amenity list, kept simple for now (no join table yet).</summary>
        public string AmenitiesCsv { get; set; }

        public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
        public virtual ICollection<RatePlan> RatePlans { get; set; } = new List<RatePlan>();
        public virtual ExtraCharge ExtraCharge { get; set; }
    }
}
