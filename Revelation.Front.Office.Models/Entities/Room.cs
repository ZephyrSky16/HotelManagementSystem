using System.Collections.Generic;
using Revelation.Front.Office.Models.Common;

namespace Revelation.Front.Office.Models.Entities
{
    public class Room : BaseEntity
    {
        public string Number { get; set; }
        public string Name { get; set; }
        public int Floor { get; set; }
        public int Capacity { get; set; }
        public string BedConfiguration { get; set; }
        public decimal BaseRate { get; set; }

        public RoomStatus Status { get; set; } = RoomStatus.Available;
        public HousekeepingStatus HousekeepingStatus { get; set; } = HousekeepingStatus.Clean;
        public bool Active { get; set; } = true;
        public bool DoNotDisturb { get; set; }

        public int RoomTypeId { get; set; }
        public virtual RoomType RoomType { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public virtual ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
        public virtual ICollection<HousekeepingLogEntry> HousekeepingLogEntries { get; set; } = new List<HousekeepingLogEntry>();
    }
}
