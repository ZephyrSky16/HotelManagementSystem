using System.Collections.Generic;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Core.Interfaces.Services
{
    public interface IRoomService
    {
        IEnumerable<Room> GetAll();
        Room GetById(int id);
        Room GetByNumber(string number);
        Room Create(Room room);
        void Update(Room room);
        void SetActive(int id, bool active);
        void UpdateStatus(int id, Models.Common.RoomStatus status, Models.Common.HousekeepingStatus housekeepingStatus);
        void RecomputeStatus(int roomId);
    }
}
