using System.Collections.Generic;
using Revelation.Front.Office.Core.Interfaces.HotellingSystem;
using Revelation.Front.Office.Models.HotellingSystem;

namespace Revelation.Front.Office.Services.HotellingSystem
{
    public class HsReservationService : IHsReservationService
    {
        private readonly IHsReservationRepository _repository;

        public HsReservationService(IHsReservationRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Reservation> GetAll() => _repository.GetAllWithLookups();

        public Reservation GetById(int id) => _repository.GetByIdWithLookups(id);

        public Reservation Create(Reservation reservation)
        {
            _repository.Add(reservation);
            _repository.SaveChanges();
            return reservation;
        }

        public void Update(Reservation reservation)
        {
            _repository.Update(reservation);
            _repository.SaveChanges();
        }

        public void Delete(int id)
        {
            var reservation = _repository.GetById(id);
            if (reservation == null) { throw new KeyNotFoundException("Reservation " + id + " not found."); }
            _repository.Remove(reservation);
            _repository.SaveChanges();
        }
    }
}
