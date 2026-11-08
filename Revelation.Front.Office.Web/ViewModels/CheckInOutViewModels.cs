using System.Collections.Generic;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Web.ViewModels
{
    public class CheckInIndexViewModel
    {
        public List<Reservation> PendingArrivals { get; set; } = new List<Reservation>();
    }

    public class CheckOutIndexViewModel
    {
        public List<Reservation> InHouseGuests { get; set; } = new List<Reservation>();
    }
}
