using System;
using System.Collections.Generic;
using Revelation.Front.Office.Core.Interfaces.Services;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Web.ViewModels
{
    public class OccupancyReportViewModel
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public List<OccupancyDataPoint> Points { get; set; } = new List<OccupancyDataPoint>();
    }

    public class BookingReportViewModel
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Status { get; set; }
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
    }

    public class RevenueByTypeDto
    {
        public string RoomTypeName { get; set; }
        public int Reservations { get; set; }
        public int RoomNights { get; set; }
        public decimal Revenue { get; set; }
        public decimal Adr { get; set; }
    }

    public class RevenueReportViewModel
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public List<RevenueByTypeDto> ByType { get; set; } = new List<RevenueByTypeDto>();
        public decimal TotalRevenue { get; set; }
        public decimal TotalBalance { get; set; }
        public int TotalNights { get; set; }
        public int TotalRooms { get; set; }
    }
}
