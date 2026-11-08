using System.Collections.Generic;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Web.ViewModels
{
    public class RoomListViewModel
    {
        public List<Room> Rooms { get; set; } = new List<Room>();
        public List<RoomType> RoomTypes { get; set; } = new List<RoomType>();
    }
}
