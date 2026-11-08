using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Revelation.Front.Office.Models.Common;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Web.ViewModels
{
    public class ReservationListViewModel
    {
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
        public List<RoomType> RoomTypes { get; set; } = new List<RoomType>();
    }

    public class ReservationFormViewModel
    {
        public int? Id { get; set; }
        public string ReservationNo { get; set; }

        // Section 1: Guest Information
        public int? ExistingGuestId { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Contact number is required.")]
        [Display(Name = "Contact Number")]
        public string Phone { get; set; }

        public string Address { get; set; }
        public string Nationality { get; set; }

        [Display(Name = "Identification Type")]
        public IdentificationType? IdType { get; set; }

        [Display(Name = "Identification Number")]
        public string IdNumber { get; set; }

        // Section 2: Reservation Details
        [Required(ErrorMessage = "Check-in date is required.")]
        [Display(Name = "Check-in Date")]
        [DataType(DataType.Date)]
        public DateTime CheckIn { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Check-out date is required.")]
        [Display(Name = "Check-out Date")]
        [DataType(DataType.Date)]
        public DateTime CheckOut { get; set; } = DateTime.Today.AddDays(1);

        [Range(1, 10)]
        public int Adults { get; set; } = 1;
        [Range(0, 10)]
        public int Children { get; set; }

        [Required(ErrorMessage = "Room type is required.")]
        [Display(Name = "Room Type")]
        public int RoomTypeId { get; set; }

        [Display(Name = "Preferred Room")]
        public int? PreferredRoomId { get; set; }

        // Section 3: Room and Rate Selection
        [Required(ErrorMessage = "Please select an available room.")]
        [Display(Name = "Assigned Room")]
        public int? SelectedRoomId { get; set; }

        public decimal Rate { get; set; }
        public decimal Discount { get; set; }

        // Section 4: Payment Information
        [Display(Name = "Amount Paid")]
        public decimal AmountPaid { get; set; }

        [Display(Name = "Payment Method")]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CreditCard;

        [Display(Name = "Payment Status")]
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

        [Display(Name = "Payment Reference")]
        public string PaymentReference { get; set; }

        // Section 5: Additional Information
        [Display(Name = "Special Requests")]
        public string SpecialRequests { get; set; }

        [Display(Name = "Internal Staff Notes")]
        public string StaffNotes { get; set; }

        [Display(Name = "Booking Source")]
        public BookingSource BookingSource { get; set; } = BookingSource.WalkIn;

        public List<RoomType> RoomTypes { get; set; } = new List<RoomType>();
        public List<Guest> Guests { get; set; } = new List<Guest>();
    }
}
