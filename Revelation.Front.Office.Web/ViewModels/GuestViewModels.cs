using System;
using System.Collections.Generic;
using System.Linq;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Web.ViewModels
{
    public class GuestListViewModel
    {
        public List<Guest> Guests { get; set; } = new List<Guest>();
        public List<IGrouping<string, Guest>> Duplicates { get; set; } = new List<IGrouping<string, Guest>>();
        public Dictionary<int, int> ReservationCounts { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, DateTime> LastVisitByGuest { get; set; } = new Dictionary<int, DateTime>();
    }
}
