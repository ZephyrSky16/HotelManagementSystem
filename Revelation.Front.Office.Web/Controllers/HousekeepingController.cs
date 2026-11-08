using System.Linq;
using System.Web.Mvc;
using Revelation.Front.Office.Core.Interfaces.Services;
using Revelation.Front.Office.Models.Common;
using Revelation.Front.Office.Services.Implementations;
using Revelation.Front.Office.Services.MockData;
using Revelation.Front.Office.Web.ViewModels;

namespace Revelation.Front.Office.Web.Controllers
{
    public class HousekeepingController : Controller
    {
        private readonly IHousekeepingService _housekeepingService;
        private readonly IRoomService _roomService;

        public HousekeepingController()
        {
            _housekeepingService = new HousekeepingService();
            _roomService = new RoomService();
        }

        public ActionResult Index()
        {
            var staff = MockDataStore.HousekeepingStaff;
            var needingCleaning = _housekeepingService.GetRoomsNeedingCleaning().ToList();
            var assignments = needingCleaning.Select((room, i) => new HousekeepingAssignmentDto
            {
                Room = room,
                AssignedTo = staff[i % staff.Count],
                AssignedTime = "09:" + (15 + i * 15).ToString("D2") + " AM"
            }).ToList();

            var vm = new HousekeepingViewModel
            {
                Rooms = _roomService.GetAll().ToList(),
                History = _housekeepingService.GetHistory().ToList(),
                Staff = staff,
                Assignments = assignments
            };
            return View(vm);
        }

        [HttpPost]
        public JsonResult UpdateRoomHousekeeping(int roomId, HousekeepingStatus status, string staff, bool doNotDisturb)
        {
            _housekeepingService.UpdateRoomHousekeeping(roomId, status, staff, doNotDisturb);
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult MarkInspected(int roomId)
        {
            _housekeepingService.UpdateRoomHousekeeping(roomId, HousekeepingStatus.Inspected, "Front Desk", false);
            return Json(new { success = true });
        }
    }
}