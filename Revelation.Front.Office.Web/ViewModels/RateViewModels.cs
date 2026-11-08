using System.Collections.Generic;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Web.ViewModels
{
    public class RateListViewModel
    {
        public List<RatePlan> RatePlans { get; set; } = new List<RatePlan>();
        public List<RoomType> RoomTypes { get; set; } = new List<RoomType>();
        public List<ExtraCharge> ExtraCharges { get; set; } = new List<ExtraCharge>();
    }
}
