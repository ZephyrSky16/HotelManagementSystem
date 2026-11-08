using System.Collections.Generic;
using Revelation.Front.Office.Models.HotellingSystem;

namespace Revelation.Front.Office.Core.Interfaces.HotellingSystem
{
    /// <summary>Reservation reads that eager-load every lookup the screens display.</summary>
    public interface IHsReservationRepository : IRepository<Reservation>
    {
        IEnumerable<Reservation> GetAllWithLookups();
        Reservation GetByIdWithLookups(int id);
    }

    /// <summary>
    /// CRUD for one HotellingSystem lookup table. Stamps the audit columns
    /// (InsertedBy/Date, UpdatedBy/Date) where the table has them.
    /// </summary>
    public interface IMasterDataService<T> where T : HsEntity
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> GetActive();
        T GetById(int id);
        T Create(T entity, string user);
        void Update(T entity, string user);
        void SetActive(int id, bool active, string user);

        /// <summary>Throws InvalidOperationException when a reservation still points at the row.</summary>
        void Delete(int id);
        bool IsInUse(int id);
    }

    public interface IHsReservationService
    {
        IEnumerable<Reservation> GetAll();
        Reservation GetById(int id);
        Reservation Create(Reservation reservation);
        void Update(Reservation reservation);
        void Delete(int id);
    }
}
