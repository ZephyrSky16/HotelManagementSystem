using System;
using System.Collections.Generic;
using System.Linq;
using Revelation.Front.Office.Core.Interfaces.Services;
using Revelation.Front.Office.Models.Common;
using Revelation.Front.Office.Services.MockData;

namespace Revelation.Front.Office.Services.Implementations
{
    public class ReportService : IReportService
    {
        public IEnumerable<OccupancyDataPoint> GetDailyOccupancy(DateTime start, DateTime end)
        {
            var totalRooms = MockDataStore.Rooms.Count;
            var points = new List<OccupancyDataPoint>();

            for (var day = start.Date; day <= end.Date; day = day.AddDays(1))
            {
                var occupied = MockDataStore.Reservations.Count(r =>
                    r.BookingStatus != BookingStatus.Cancelled && r.BookingStatus != BookingStatus.NoShow &&
                    r.RoomId != null && day >= r.CheckIn.Date && day < r.CheckOut.Date);

                points.Add(new OccupancyDataPoint
                {
                    Date = day,
                    RoomsOccupied = occupied,
                    RoomsAvailable = totalRooms - occupied,
                    TotalRooms = totalRooms,
                    OccupancyPercent = totalRooms > 0 ? Math.Round((decimal)occupied / totalRooms * 100, 1) : 0
                });
            }
            return points;
        }
    }
}
