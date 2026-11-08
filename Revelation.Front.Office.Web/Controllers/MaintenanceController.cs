using System.Linq;
using System.Web.Mvc;
using Revelation.Front.Office.Core.Interfaces.Services;
using Revelation.Front.Office.Models.Common;
using Revelation.Front.Office.Models.Entities;
using Revelation.Front.Office.Services.Implementations;
using Revelation.Front.Office.Services.MockData;
using Revelation.Front.Office.Web.ViewModels;

namespace Revelation.Front.Office.Web.Controllers
{
    public class MaintenanceController : Controller
    {
        private readonly IMaintenanceService _maintenanceService;
        private readonly IRoomService _roomService;

        public MaintenanceController()
        {
            _roomService = new RoomService();
            _maintenanceService = new MaintenanceService(_roomService);
        }

        public ActionResult Index()
        {
            var vm = new MaintenanceListViewModel
            {
                Requests = _maintenanceService.GetAll().ToList(),
                Rooms = _roomService.GetAll().ToList(),
                Personnel = MockDataStore.MaintenancePersonnel
            };
            return View(vm);
        }

        [HttpPost]
        public JsonResult CreateRequest(int roomId, MaintenanceCategory category, string description, MaintenancePriority priority, string reportedBy, string assignedPersonnel, string expectedCompletion, bool markRoomOutOfOrder)
        {
            var request = new MaintenanceRequest
            {
                RoomId = roomId, Category = category, Description = description, Priority = priority,
                ReportedBy = reportedBy, AssignedPersonnel = assignedPersonnel,
                ExpectedCompletion = string.IsNullOrEmpty(expectedCompletion) ? (System.DateTime?)null : System.DateTime.Parse(expectedCompletion),
                MarkedRoomOutOfOrder = markRoomOutOfOrder
            };
            _maintenanceService.Create(request);
            return Json(new { success = true, requestNo = request.RequestNo });
        }

        [HttpPost]
        public JsonResult UpdateStatus(int id, MaintenanceStatus status)
        {
            _maintenanceService.UpdateStatus(id, status);
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult AssignPersonnel(int id, string personnel)
        {
            _maintenanceService.AssignPersonnel(id, personnel);
            return Json(new { success = true });
        }
    }
}
