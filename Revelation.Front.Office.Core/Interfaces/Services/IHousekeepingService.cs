using System.Collections.Generic;
using Revelation.Front.Office.Models.Entities;
using Revelation.Front.Office.Models.Common;

namespace Revelation.Front.Office.Core.Interfaces.Services
{
    public interface IHousekeepingService
    {
        IEnumerable<Room> GetRoomsNeedingCleaning();
        IEnumerable<HousekeepingLogEntry> GetHistory(int take = 20);
        void UpdateRoomHousekeeping(int roomId, HousekeepingStatus status, string staff, bool doNotDisturb);
    }
}
