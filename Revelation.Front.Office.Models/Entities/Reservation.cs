using System;
using Revelation.Front.Office.Models.Common;

namespace Revelation.Front.Office.Models.Entities
{
    public class Reservation : BaseEntity
    {
        public string ReservationNo { get; set; }

        public int GuestId { get; set; }
        public virtual Guest Guest { get; set; }

        public int RoomTypeId { get; set; }
        public virtual RoomType RoomType { get; set; }

        public int? RoomId { get; set; }
        public virtual Room Room { get; set; }

        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Adults { get; set; } = 1;
        public int Children { get; set; }
        public int NumRooms { get; set; } = 1;

        public decimal Rate { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ExtraCharges { get; set; }
        public decimal TaxesAndFees { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Balance { get; set; }

        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CreditCard;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;
        public string PaymentReference { get; set; }

        public BookingStatus BookingStatus { get; set; } = BookingStatus.Confirmed;
        public BookingSource BookingSource { get; set; } = BookingSource.WalkIn;

        public string SpecialRequests { get; set; }
        public string StaffNotes { get; set; }

        public int Nights => Math.Max(1, (CheckOut.Date - CheckIn.Date).Days);
    }
}
