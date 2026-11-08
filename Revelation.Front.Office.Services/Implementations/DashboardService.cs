using System;
using System.Linq;
using Revelation.Front.Office.Core.Interfaces.Services;
using Revelation.Front.Office.Models.Common;
using Revelation.Front.Office.Services.MockData;

namespace Revelation.Front.Office.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        public DashboardSummary GetSummary()
        {
            var rooms = MockDataStore.Rooms;
            var today = DateTime.Today;

            return new DashboardSummary
            {
                TotalRooms = rooms.Count,
                AvailableRooms = rooms.Count(r => r.Status == RoomStatus.Available),
                OccupiedRooms = rooms.Count(r => r.Status == RoomStatus.Occupied),
                ReservedRooms = rooms.Count(r => r.Status == RoomStatus.Reserved),
                RoomsUnderMaintenance = rooms.Count(r => r.Status == RoomStatus.UnderMaintenance || r.Status == RoomStatus.OutOfOrder),
                TodaysCheckIns = MockDataStore.Reservations.Count(r => r.CheckIn.Date == today &&
                    (r.BookingStatus == BookingStatus.Confirmed || r.BookingStatus == BookingStatus.Tentative || r.BookingStatus == BookingStatus.CheckedIn)),
                TodaysCheckOuts = MockDataStore.Reservations.Count(r => r.CheckOut.Date == today &&
                    (r.BookingStatus == BookingStatus.CheckedIn || r.BookingStatus == BookingStatus.CheckedOut))
            };
        }
    }
}
