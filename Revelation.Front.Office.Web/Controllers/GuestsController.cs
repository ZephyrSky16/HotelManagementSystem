using System.Linq;
using System.Web.Mvc;
using Revelation.Front.Office.Core.Interfaces.Services;
using Revelation.Front.Office.Services.Implementations;
using Revelation.Front.Office.Web.ViewModels;

namespace Revelation.Front.Office.Web.Controllers
{
    public class GuestsController : Controller
    {
        private readonly IGuestService _guestService;
        private readonly IReservationService _reservationService;
        private readonly IRoomService _roomService;

        public GuestsController()
        {
            _guestService = new GuestService();
            _roomService = new RoomService();
            _reservationService = new ReservationService(_roomService);
        }

        public ActionResult Index()
        {
            var reservations = _reservationService.GetAll().ToList();
            var vm = new GuestListViewModel
            {
                Guests = _guestService.GetAll().ToList(),
                Duplicates = _guestService.FindPossibleDuplicates().ToList(),
                ReservationCounts = reservations.GroupBy(r => r.GuestId).ToDictionary(g => g.Key, g => g.Count()),
                LastVisitByGuest = reservations.Where(r => r.BookingStatus == Revelation.Front.Office.Models.Common.BookingStatus.CheckedOut)
                    .GroupBy(r => r.GuestId).ToDictionary(g => g.Key, g => g.Max(r => r.CheckOut))
            };
            return View(vm);
        }

        [HttpGet]
        public PartialViewResult GetGuestDetails(int id)
        {
            var guest = _guestService.GetById(id);
            var history = _reservationService.GetAll().Where(r => r.GuestId == id).OrderByDescending(r => r.CheckIn).ToList();
            ViewBag.History = history;
            ViewBag.TotalReservations = history.Count;
            return PartialView("_GuestDetails", guest);
        }
    }
}
