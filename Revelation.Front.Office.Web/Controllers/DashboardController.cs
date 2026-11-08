using System;
using System.Linq;
using System.Web.Mvc;
using Revelation.Front.Office.Core.Interfaces.Services;
using Revelation.Front.Office.Models.Common;
using Revelation.Front.Office.Services.Implementations;
using Revelation.Front.Office.Web.ViewModels;

namespace Revelation.Front.Office.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly IReservationService _reservationService;
        private readonly IRoomService _roomService;

        public DashboardController()
        {
            _roomService = new RoomService();
            _reservationService = new ReservationService(_roomService);
            _dashboardService = new DashboardService();
        }

        public ActionResult Index()
        {
            var rooms = _roomService.GetAll().ToList();

            var vm = new DashboardViewModel
            {
                Summary = _dashboardService.GetSummary(),
                TodaysArrivals = _reservationService.GetTodaysArrivals().ToList(),
                TodaysDepartures = _reservationService.GetTodaysDepartures().ToList(),
                RecentReservations = _reservationService.GetAll().Take(7).ToList(),
                OccupiedCount = rooms.Count(r => r.Status == RoomStatus.Occupied),
                ReservedCount = rooms.Count(r => r.Status == RoomStatus.Reserved),
                AvailableCount = rooms.Count(r => r.Status == RoomStatus.Available)
            };
            vm.OtherCount = rooms.Count - vm.OccupiedCount - vm.ReservedCount - vm.AvailableCount;

            return View(vm);
        }

        [HttpPost]
        public ActionResult QuickCheckIn(int reservationId)
        {
            _reservationService.CheckIn(reservationId);
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult QuickCheckOut(int reservationId)
        {
            var res = _reservationService.GetById(reservationId);
            if (res != null && res.Balance > 0)
            {
                return Json(new { success = false, message = "Outstanding balance of " + res.Balance.ToString("C") + " -- settle payment on the Check-out page." });
            }
            _reservationService.CheckOut(reservationId);
            return Json(new { success = true });
        }
    }
}
