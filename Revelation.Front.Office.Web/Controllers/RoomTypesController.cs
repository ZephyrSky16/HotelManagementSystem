using System.Linq;
using System.Web.Mvc;
using Revelation.Front.Office.Core.Interfaces.Services;
using Revelation.Front.Office.Models.Entities;
using Revelation.Front.Office.Services.Implementations;

namespace Revelation.Front.Office.Web.Controllers
{
    public class RoomTypesController : Controller
    {
        private readonly IRoomTypeService _roomTypeService;
        private readonly IRoomService _roomService;

        public RoomTypesController()
        {
            _roomTypeService = new RoomTypeService();
            _roomService = new RoomService();
        }

        public ActionResult Index()
        {
            ViewBag.RoomCounts = _roomService.GetAll().GroupBy(r => r.RoomTypeId).ToDictionary(g => g.Key, g => g.Count());
            return View(_roomTypeService.GetAll().ToList());
        }

        [HttpPost]
        public JsonResult SaveRoomType(int? id, string name, string description, int maxOccupancy, int maxAdults, int maxChildren, string bedConfiguration, decimal baseRate, string amenitiesCsv, bool active)
        {
            if (string.IsNullOrWhiteSpace(name) || baseRate <= 0) { return Json(new { success = false, message = "Name and base rate are required." }); }

            if (id.HasValue)
            {
                var type = _roomTypeService.GetById(id.Value);
                if (type == null) { return Json(new { success = false, message = "Room type not found." }); }
                type.Name = name; type.Description = description; type.MaxOccupancy = maxOccupancy; type.MaxAdults = maxAdults;
                type.MaxChildren = maxChildren; type.BedConfiguration = bedConfiguration; type.BaseRate = baseRate;
                type.AmenitiesCsv = amenitiesCsv; type.Active = active;
                _roomTypeService.Update(type);
            }
            else
            {
                _roomTypeService.Create(new RoomType
                {
                    Name = name, Description = description, MaxOccupancy = maxOccupancy, MaxAdults = maxAdults,
                    MaxChildren = maxChildren, BedConfiguration = bedConfiguration, BaseRate = baseRate, AmenitiesCsv = amenitiesCsv, Active = active
                });
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult ToggleActive(int id)
        {
            var type = _roomTypeService.GetById(id);
            if (type == null) { return Json(new { success = false }); }
            _roomTypeService.SetActive(id, !type.Active);
            return Json(new { success = true, active = type.Active });
        }
    }
}
