using System;
using System.Linq;
using System.Web.Mvc;
using Revelation.Front.Office.Core.Interfaces.Services;
using Revelation.Front.Office.Models.Common;
using Revelation.Front.Office.Models.Entities;
using Revelation.Front.Office.Services.Implementations;
using Revelation.Front.Office.Web.ViewModels;

namespace Revelation.Front.Office.Web.Controllers
{
    public class RoomsController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IRoomTypeService _roomTypeService;
        private readonly IReservationService _reservationService;

        public RoomsController()
        {
            _roomService = new RoomService();
            _roomTypeService = new RoomTypeService();
            _reservationService = new ReservationService(_roomService);
        }

        public ActionResult Index()
        {
            var vm = new RoomListViewModel
            {
                Rooms = _roomService.GetAll().ToList(),
                RoomTypes = _roomTypeService.GetAll().ToList()
            };
            return View(vm);
        }

        public ActionResult Status()
        {
            var vm = new RoomListViewModel
            {
                Rooms = _roomService.GetAll().ToList(),
                RoomTypes = _roomTypeService.GetAll().ToList()
            };
            return View(vm);
        }

        [HttpPost]
        public JsonResult SaveRoom(int? id, string number, string name, int roomTypeId, int floor, int capacity, string bedConfiguration, decimal baseRate, RoomStatus status, bool active)
        {
            var type = _roomTypeService.GetById(roomTypeId);
            if (type == null) { return Json(new { success = false, message = "Invalid room type." }); }

            if (id.HasValue)
            {
                var room = _roomService.GetById(id.Value);
                if (room == null) { return Json(new { success = false, message = "Room not found." }); }
                room.Number = number; room.Name = name; room.RoomTypeId = roomTypeId; room.RoomType = type;
                room.Floor = floor; room.Capacity = capacity; room.BedConfiguration = bedConfiguration;
                room.BaseRate = baseRate; room.Status = status; room.Active = active;
                _roomService.Update(room);
            }
            else
            {
                if (_roomService.GetByNumber(number) != null) { return Json(new { success = false, message = "Room " + number + " already exists." }); }
                _roomService.Create(new Room
                {
                    Number = number, Name = name, RoomTypeId = roomTypeId, RoomType = type, Floor = floor,
                    Capacity = capacity, BedConfiguration = bedConfiguration, BaseRate = baseRate, Status = status, Active = active
                });
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult ToggleActive(int id)
        {
            var room = _roomService.GetById(id);
            if (room == null) { return Json(new { success = false }); }
            _roomService.SetActive(id, !room.Active);
            return Json(new { success = true, active = room.Active });
        }

        [HttpPost]
        public JsonResult UpdateStatus(int id, RoomStatus status, HousekeepingStatus housekeepingStatus)
        {
            _roomService.UpdateStatus(id, status, housekeepingStatus);
            return Json(new { success = true });
        }

        [HttpGet]
        public PartialViewResult GetRoomDetails(int id)
        {
            var room = _roomService.GetById(id);
            ViewBag.Current = _reservationService.GetAll().FirstOrDefault(r => r.RoomId == id && r.BookingStatus == BookingStatus.CheckedIn);
            ViewBag.History = _reservationService.GetAll()
                .Where(r => r.RoomId == id && (r.BookingStatus == BookingStatus.CheckedOut || r.BookingStatus == BookingStatus.Cancelled || r.BookingStatus == BookingStatus.NoShow))
                .OrderByDescending(r => r.CheckIn).Take(5).ToList();
            return PartialView("_RoomDetails", room);
        }
    }
}
