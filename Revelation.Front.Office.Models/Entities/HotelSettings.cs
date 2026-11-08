using Revelation.Front.Office.Models.Common;

namespace Revelation.Front.Office.Models.Entities
{
    /// <summary>Single-row configuration record for the property.</summary>
    public class HotelSettings : BaseEntity
    {
        public string HotelName { get; set; }
        public string LegalName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string CheckInTime { get; set; }
        public string CheckOutTime { get; set; }
        public string Currency { get; set; }
        public string Timezone { get; set; }
        public decimal TaxRate { get; set; }
        public decimal ServiceCharge { get; set; }
    }
}
