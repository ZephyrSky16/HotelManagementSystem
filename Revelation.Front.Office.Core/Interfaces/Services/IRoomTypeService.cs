using System.Collections.Generic;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Core.Interfaces.Services
{
    public interface IRoomTypeService
    {
        IEnumerable<RoomType> GetAll();
        RoomType GetById(int id);
        RoomType Create(RoomType roomType);
        void Update(RoomType roomType);
        void SetActive(int id, bool active);
    }
}
