using System;
using System.Collections.Generic;
using System.Linq;
using Revelation.Front.Office.Core.Interfaces.Services;
using Revelation.Front.Office.Models.Entities;
using Revelation.Front.Office.Services.MockData;

namespace Revelation.Front.Office.Services.Implementations
{
    public class RoomTypeService : IRoomTypeService
    {
        public IEnumerable<RoomType> GetAll() => MockDataStore.RoomTypes;

        public RoomType GetById(int id) => MockDataStore.RoomTypes.FirstOrDefault(t => t.Id == id);

        public RoomType Create(RoomType roomType)
        {
            roomType.Id = MockDataStore.NextId();
            roomType.Active = true;
            MockDataStore.RoomTypes.Add(roomType);
            return roomType;
        }

        public void Update(RoomType roomType)
        {
            var existing = GetById(roomType.Id);
            if (existing == null) return;

            existing.Name = roomType.Name;
            existing.Description = roomType.Description;
            existing.MaxOccupancy = roomType.MaxOccupancy;
            existing.MaxAdults = roomType.MaxAdults;
            existing.MaxChildren = roomType.MaxChildren;
            existing.BedConfiguration = roomType.BedConfiguration;
            existing.BaseRate = roomType.BaseRate;
            existing.AmenitiesCsv = roomType.AmenitiesCsv;
            existing.ModifiedDate = DateTime.Now;
        }

        public void SetActive(int id, bool active)
        {
            var type = GetById(id);
            if (type == null) return;
            type.Active = active;
            type.ModifiedDate = DateTime.Now;
        }
    }
}
