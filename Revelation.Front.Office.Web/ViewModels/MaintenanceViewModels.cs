using System.Collections.Generic;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Web.ViewModels
{
    public class MaintenanceListViewModel
    {
        public List<MaintenanceRequest> Requests { get; set; } = new List<MaintenanceRequest>();
        public List<Room> Rooms { get; set; } = new List<Room>();
        public List<string> Personnel { get; set; } = new List<string>();
    }
}
