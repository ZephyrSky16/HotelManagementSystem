using System;
using System.Collections.Generic;

namespace Revelation.Front.Office.Core.Interfaces.Services
{
    public class OccupancyDataPoint
    {
        public DateTime Date { get; set; }
        public int RoomsOccupied { get; set; }
        public int RoomsAvailable { get; set; }
        public int TotalRooms { get; set; }
        public decimal OccupancyPercent { get; set; }
    }

    public interface IReportService
    {
        IEnumerable<OccupancyDataPoint> GetDailyOccupancy(DateTime start, DateTime end);
    }
}
