using System.Collections.Generic;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Web.ViewModels
{
    public class HousekeepingAssignmentDto
    {
        public Room Room { get; set; }
        public string AssignedTo { get; set; }
        public string AssignedTime { get; set; }
    }

    public class HousekeepingViewModel
    {
        public List<Room> Rooms { get; set; } = new List<Room>();
        public List<HousekeepingLogEntry> History { get; set; } = new List<HousekeepingLogEntry>();
        public List<string> Staff { get; set; } = new List<string>();
        public List<HousekeepingAssignmentDto> Assignments { get; set; } = new List<HousekeepingAssignmentDto>();
    }
}
