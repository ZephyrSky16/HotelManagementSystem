using System.Linq;
using Revelation.Front.Office.Core.Interfaces.Repositories;
using Revelation.Front.Office.Models.Entities;

namespace Revelation.Front.Office.Repositories.EF
{
    public class HotelSettingsRepository : IHotelSettingsRepository
    {
        private readonly RevelationDbContext _context;

        public HotelSettingsRepository(RevelationDbContext context)
        {
            _context = context;
        }

        public HotelSettings Get() => _context.HotelSettings.FirstOrDefault();

        public void Save(HotelSettings settings)
        {
            if (settings.Id == 0)
            {
                _context.HotelSettings.Add(settings);
            }
            else
            {
                _context.Entry(settings).State = System.Data.Entity.EntityState.Modified;
            }
            _context.SaveChanges();
        }
    }
}
