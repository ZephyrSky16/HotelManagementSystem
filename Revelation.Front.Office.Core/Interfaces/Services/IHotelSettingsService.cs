using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Core.Interfaces.Services
{
    public interface IHotelSettingsService
    {
        HotelSettings Get();
        void Save(HotelSettings settings);
    }
}
