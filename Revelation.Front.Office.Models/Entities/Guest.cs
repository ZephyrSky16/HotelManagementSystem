using System;
using System.Collections.Generic;
using Revelation.Front.Office.Models.Common;

namespace Revelation.Front.Office.Models.Entities
{
    public class Guest : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Nationality { get; set; }
        public IdentificationType? IdType { get; set; }
        public string IdNumber { get; set; }
        public string InternalNotes { get; set; }
        public string SpecialRequests { get; set; }
        public DateTime? LastVisit { get; set; }

        public string FullName => $"{FirstName} {LastName}".Trim();

        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
