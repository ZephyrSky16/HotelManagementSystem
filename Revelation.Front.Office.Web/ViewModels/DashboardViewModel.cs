using System.Collections.Generic;
using Revelation.Front.Office.Core.Interfaces.Services;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Web.ViewModels
{
    public class DashboardViewModel
    {
        public DashboardSummary Summary { get; set; }
        public List<Reservation> TodaysArrivals { get; set; } = new List<Reservation>();
        public List<Reservation> TodaysDepartures { get; set; } = new List<Reservation>();
        public List<Reservation> RecentReservations { get; set; } = new List<Reservation>();
        public int OccupiedCount { get; set; }
        public int ReservedCount { get; set; }
        public int AvailableCount { get; set; }
        public int OtherCount { get; set; }
    }
}
