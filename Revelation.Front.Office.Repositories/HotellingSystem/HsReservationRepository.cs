using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Revelation.Front.Office.Core.Interfaces.HotellingSystem;
using Revelation.Front.Office.Models.HotellingSystem;
using Revelation.Front.Office.Repositories.EF.Generic;

namespace Revelation.Front.Office.Repositories.HotellingSystem
{
    public class HsReservationRepository : EfRepository<Reservation>, IHsReservationRepository
    {
        public HsReservationRepository(HotellingSystemDbContext context) : base(context) { }

        private IQueryable<Reservation> WithLookups() =>
            Set.Include(r => r.Agent)
               .Include(r => r.ConfirmTo)
               .Include(r => r.ConfirmVia)
               .Include(r => r.LastCancellation)
               .Include(r => r.MarketCode)
               .Include(r => r.Rate)
               .Include(r => r.ReservationStatus)
               .Include(r => r.Sales)
               .Include(r => r.BookingCode);

        public IEnumerable<Reservation> GetAllWithLookups() =>
            WithLookups().OrderByDescending(r => r.Id).ToList();

        public Reservation GetByIdWithLookups(int id) =>
            WithLookups().FirstOrDefault(r => r.Id == id);
    }
}
