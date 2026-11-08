using System.Collections.Generic;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Web.ViewModels
{
    public class CalendarIndexViewModel
    {
        public List<RoomType> RoomTypes { get; set; } = new List<RoomType>();
        public List<int> Floors { get; set; } = new List<int>();
    }

    public class RackRoomDto
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Type { get; set; }
        public int Floor { get; set; }
        public string Status { get; set; }
        public string HousekeepingStatus { get; set; }
    }

    public class RackReservationDto
    {
        public int Id { get; set; }
        public string ReservationNo { get; set; }
        public string GuestName { get; set; }
        public string ContactNumber { get; set; }
        public int? RoomId { get; set; }
        public string RoomType { get; set; }
        public int RoomTypeId { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public string BookingStatus { get; set; }
        public string PaymentStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Balance { get; set; }
        public int Nights { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
    }
}
