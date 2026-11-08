using Revelation.Front.Office.Core.Interfaces.Services;
using Revelation.Front.Office.Models.Entities;
using Revelation.Front.Office.Services.MockData;

namespace Revelation.Front.Office.Services.Implementations
{
    public class HotelSettingsService : IHotelSettingsService
    {
        public HotelSettings Get() => MockDataStore.Settings;

        public void Save(HotelSettings settings)
        {
            var existing = MockDataStore.Settings;
            existing.HotelName = settings.HotelName;
            existing.LegalName = settings.LegalName;
            existing.Address = settings.Address;
            existing.Phone = settings.Phone;
            existing.Email = settings.Email;
            existing.Website = settings.Website;
            existing.CheckInTime = settings.CheckInTime;
            existing.CheckOutTime = settings.CheckOutTime;
            existing.Currency = settings.Currency;
            existing.Timezone = settings.Timezone;
            existing.TaxRate = settings.TaxRate;
            existing.ServiceCharge = settings.ServiceCharge;
        }
    }
}
