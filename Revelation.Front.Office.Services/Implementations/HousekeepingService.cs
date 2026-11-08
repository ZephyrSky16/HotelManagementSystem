using System;
using System.Collections.Generic;
using System.Linq;
using Revelation.Front.Office.Core.Interfaces.Services;
using Revelation.Front.Office.Models.Common;
using Revelation.Front.Office.Models.Entities;
using Revelation.Front.Office.Services.MockData;

namespace Revelation.Front.Office.Services.Implementations
{
    public class HousekeepingService : IHousekeepingService
    {
        public IEnumerable<Room> GetRoomsNeedingCleaning() =>
            MockDataStore.Rooms.Where(r => r.HousekeepingStatus == HousekeepingStatus.Dirty);

        public IEnumerable<HousekeepingLogEntry> GetHistory(int take = 20) =>
            MockDataStore.HousekeepingLog.OrderByDescending(h => h.EntryDate).Take(take);

        public void UpdateRoomHousekeeping(int roomId, HousekeepingStatus status, string staff, bool doNotDisturb)
        {
            var room = MockDataStore.Rooms.FirstOrDefault(r => r.Id == roomId);
            if (room == null) return;

            var fromStatus = room.HousekeepingStatus;
            room.HousekeepingStatus = status;
            room.DoNotDisturb = doNotDisturb;

            if (fromStatus != status)
            {
                MockDataStore.HousekeepingLog.Insert(0, new HousekeepingLogEntry
                {
                    Id = MockDataStore.NextId(),
                    RoomId = room.Id,
                    Room = room,
                    Staff = string.IsNullOrEmpty(staff) ? "Unassigned" : staff,
                    EntryDate = DateTime.Now,
                    FromStatus = fromStatus,
                    ToStatus = status,
                    DurationMinutes = 20 + new Random().Next(0, 25)
                });
            }
        }
    }
}
