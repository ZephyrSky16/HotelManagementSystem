using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Core.Interfaces.Repositories
{
    public interface IHotelSettingsRepository
    {
        HotelSettings Get();
        void Save(HotelSettings settings);
    }
}
