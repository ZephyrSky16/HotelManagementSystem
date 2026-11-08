using System;
using System.Collections.Generic;
using System.Linq;
using Revelation.Front.Office.Core.Interfaces.Services;
using Revelation.Front.Office.Models.Common;
using Revelation.Front.Office.Models.Entities;
using Revelation.Front.Office.Services.MockData;

namespace Revelation.Front.Office.Services.Implementations
{
    public class RoomService : IRoomService
    {
        public IEnumerable<Room> GetAll() => MockDataStore.Rooms.OrderBy(r => r.Number);

        public Room GetById(int id) => MockDataStore.Rooms.FirstOrDefault(r => r.Id == id);

        public Room GetByNumber(string number) => MockDataStore.Rooms.FirstOrDefault(r => r.Number == number);

        public Room Create(Room room)
        {
            room.Id = MockDataStore.NextId();
            room.HousekeepingStatus = HousekeepingStatus.Clean;
            MockDataStore.Rooms.Add(room);
            return room;
        }

        public void Update(Room room)
        {
            var existing = GetById(room.Id);
            if (existing == null) return;

            existing.Number = room.Number;
            existing.Name = room.Name;
            existing.Floor = room.Floor;
            existing.RoomTypeId = room.RoomTypeId;
            existing.RoomType = room.RoomType;
            existing.Capacity = room.Capacity;
            existing.BedConfiguration = room.BedConfiguration;
            existing.BaseRate = room.BaseRate;
            existing.Status = room.Status;
            existing.ModifiedDate = DateTime.Now;
        }

        public void SetActive(int id, bool active)
        {
            var room = GetById(id);
            if (room == null) return;

            room.Active = active;
            if (!active && room.Status == RoomStatus.Available)
            {
                room.Status = RoomStatus.OutOfOrder;
            }
            room.ModifiedDate = DateTime.Now;
        }

        public void UpdateStatus(int id, RoomStatus status, HousekeepingStatus housekeepingStatus)
        {
            var room = GetById(id);
            if (room == null) return;

            room.Status = status;
            room.HousekeepingStatus = housekeepingStatus;
            room.ModifiedDate = DateTime.Now;
        }

        public void RecomputeStatus(int roomId)
        {
            var room = GetById(roomId);
            if (room == null || room.Status == RoomStatus.OutOfOrder || room.Status == RoomStatus.UnderMaintenance)
            {
                return;
            }

            var today = DateTime.Today;
            var current = MockDataStore.Reservations.FirstOrDefault(r => r.RoomId == roomId && r.BookingStatus == BookingStatus.CheckedIn);
            if (current != null)
            {
                room.Status = RoomStatus.Occupied;
                room.HousekeepingStatus = HousekeepingStatus.Occupied;
                return;
            }

            var upcoming = MockDataStore.Reservations.FirstOrDefault(r => r.RoomId == roomId &&
                (r.BookingStatus == BookingStatus.Confirmed || r.BookingStatus == BookingStatus.Tentative) &&
                r.CheckOut > today);
            room.Status = upcoming != null ? RoomStatus.Reserved : RoomStatus.Available;
        }
    }
}
