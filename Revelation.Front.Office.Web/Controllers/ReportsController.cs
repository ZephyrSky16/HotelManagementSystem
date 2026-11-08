using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Revelation.Front.Office.Core.Interfaces.Services;
using Revelation.Front.Office.Models.Common;
using Revelation.Front.Office.Services.Implementations;
using Revelation.Front.Office.Web.ViewModels;

namespace Revelation.Front.Office.Web.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IReservationService _reservationService;
        private readonly IRoomService _roomService;
        private readonly IRoomTypeService _roomTypeService;

        public ReportsController()
        {
            _roomService = new RoomService();
            _reservationService = new ReservationService(_roomService);
            _reportService = new ReportService();
            _roomTypeService = new RoomTypeService();
        }

        private DateTime ParseOrDefault(string s, DateTime fallback)
        {
            DateTime d;
            return DateTime.TryParse(s, out d) ? d : fallback;
        }

        public ActionResult Occupancy(string start, string end)
        {
            var startDate = ParseOrDefault(start, DateTime.Today.AddDays(-29));
            var endDate = ParseOrDefault(end, DateTime.Today);
            var vm = new OccupancyReportViewModel
            {
                Start = startDate, End = endDate,
                Points = _reportService.GetDailyOccupancy(startDate, endDate).ToList()
            };
            return View(vm);
        }

        public FileResult ExportOccupancyCsv(string start, string end)
        {
            var startDate = ParseOrDefault(start, DateTime.Today.AddDays(-29));
            var endDate = ParseOrDefault(end, DateTime.Today);
            var points = _reportService.GetDailyOccupancy(startDate, endDate);

            var sb = new StringBuilder();
            sb.AppendLine("Date,Rooms Occupied,Rooms Available,Total Rooms,Occupancy %");
            foreach (var p in points)
            {
                sb.AppendLine(p.Date.ToString("yyyy-MM-dd") + "," + p.RoomsOccupied + "," + p.RoomsAvailable + "," + p.TotalRooms + "," + p.OccupancyPercent);
            }
            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "occupancy-report.csv");
        }

        public ActionResult Booking(string start, string end, string status)
        {
            var startDate = ParseOrDefault(start, DateTime.Today.AddDays(-60));
            var endDate = ParseOrDefault(end, DateTime.Today.AddDays(10));

            var list = _reservationService.GetAll().Where(r => r.CreatedDate.Date >= startDate.Date && r.CreatedDate.Date <= endDate.Date);
            if (!string.IsNullOrEmpty(status))
            {
                BookingStatus parsed;
                if (Enum.TryParse(status, out parsed)) { list = list.Where(r => r.BookingStatus == parsed); }
            }

            var vm = new BookingReportViewModel { Start = startDate, End = endDate, Status = status, Reservations = list.ToList() };
            return View(vm);
        }

        public ActionResult Revenue(string start, string end)
        {
            var startDate = ParseOrDefault(start, DateTime.Today.AddDays(-60));
            var endDate = ParseOrDefault(end, DateTime.Today.AddDays(10));

            var list = _reservationService.GetAll()
                .Where(r => r.BookingStatus != BookingStatus.Cancelled && r.CheckIn.Date <= endDate.Date && r.CheckOut.Date >= startDate.Date)
                .ToList();

            var byType = _roomTypeService.GetAll().Select(t =>
            {
                var rs = list.Where(r => r.RoomTypeId == t.Id).ToList();
                var revenue = rs.Sum(r => r.TotalAmount);
                var nights = rs.Sum(r => r.Nights);
                return new RevenueByTypeDto { RoomTypeName = t.Name, Reservations = rs.Count, RoomNights = nights, Revenue = revenue, Adr = nights > 0 ? revenue / nights : 0 };
            }).ToList();

            var vm = new RevenueReportViewModel
            {
                Start = startDate, End = endDate, ByType = byType,
                TotalRevenue = list.Sum(r => r.TotalAmount), TotalBalance = list.Sum(r => r.Balance),
                TotalNights = list.Sum(r => r.Nights), TotalRooms = _roomService.GetAll().Count()
            };
            return View(vm);
        }
    }
}
