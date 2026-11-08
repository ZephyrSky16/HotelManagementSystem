using System;
using Revelation.Front.Office.Models.Common;

namespace Revelation.Front.Office.Models.Entities
{
    public class MaintenanceRequest : BaseEntity
    {
        public string RequestNo { get; set; }

        public int RoomId { get; set; }
        public virtual Room Room { get; set; }

        public MaintenanceCategory Category { get; set; }
        public string Description { get; set; }
        public MaintenancePriority Priority { get; set; } = MaintenancePriority.Medium;
        public string ReportedBy { get; set; }
        public string AssignedPersonnel { get; set; }

        public DateTime ReportedDate { get; set; } = DateTime.Now;
        public DateTime? ExpectedCompletion { get; set; }
        public DateTime? ActualCompletion { get; set; }

        public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Reported;
        public bool MarkedRoomOutOfOrder { get; set; }
    }
}
