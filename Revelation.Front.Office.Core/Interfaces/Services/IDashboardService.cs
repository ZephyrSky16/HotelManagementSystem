namespace Revelation.Front.Office.Core.Interfaces.Services
{
    public class DashboardSummary
    {
        public int TotalRooms { get; set; }
        public int AvailableRooms { get; set; }
        public int OccupiedRooms { get; set; }
        public int ReservedRooms { get; set; }
        public int RoomsUnderMaintenance { get; set; }
        public int TodaysCheckIns { get; set; }
        public int TodaysCheckOuts { get; set; }
    }

    public interface IDashboardService
    {
        DashboardSummary GetSummary();
    }
}
