using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Revelation.Front.Office.Core.Interfaces;
using Revelation.Front.Office.Core.Interfaces.HotellingSystem;
using Revelation.Front.Office.Models.HotellingSystem;

namespace Revelation.Front.Office.Services.HotellingSystem
{
    public class MasterDataService<T> : IMasterDataService<T> where T : HsEntity
    {
        private readonly IRepository<T> _repository;
        private readonly IRepository<Reservation> _reservations;
        private readonly Func<int, Expression<Func<Reservation, bool>>> _usedBy;

        /// <param name="usedBy">
        /// Builds the "reservation references this row" predicate for an Id,
        /// e.g. id =&gt; r =&gt; r.AgentId == id. Null when nothing references the table.
        /// </param>
        public MasterDataService(IRepository<T> repository, IRepository<Reservation> reservations,
            Func<int, Expression<Func<Reservation, bool>>> usedBy)
        {
            _repository = repository;
            _reservations = reservations;
            _usedBy = usedBy;
        }

        public IEnumerable<T> GetAll() => _repository.Query().OrderBy(e => e.Id).ToList();

        public IEnumerable<T> GetActive() =>
            GetAll().Where(e => !(e is IActivatable a) || a.IsActive != false).ToList();

        public T GetById(int id) => _repository.GetById(id);

        public T Create(T entity, string user)
        {
            var now = DateTime.Now;
            if (entity is IInsertAudited inserted) { inserted.InsertedDate = now; inserted.InsertedBy = user; }
            if (entity is IActivatable activatable && !activatable.IsActive.HasValue) { activatable.IsActive = true; }
            _repository.Add(entity);
            _repository.SaveChanges();
            return entity;
        }

        public void Update(T entity, string user)
        {
            if (entity is IUpdateAudited updated) { updated.UpdatedDate = DateTime.Now; updated.UpdatedBy = user; }
            _repository.Update(entity);
            _repository.SaveChanges();
        }

        public void SetActive(int id, bool active, string user)
        {
            var entity = _repository.GetById(id);
            if (entity == null) { throw new KeyNotFoundException("Record " + id + " not found."); }
            if (!(entity is IActivatable activatable)) { throw new InvalidOperationException("This table has no IsActive column."); }
            activatable.IsActive = active;
            Update(entity, user);
        }

        public bool IsInUse(int id) => _usedBy != null && _reservations.Query().Any(_usedBy(id));

        public void Delete(int id)
        {
            var entity = _repository.GetById(id);
            if (entity == null) { throw new KeyNotFoundException("Record " + id + " not found."); }
            if (IsInUse(id))
            {
                throw new InvalidOperationException("This record is used by one or more reservations. Deactivate it instead.");
            }
            _repository.Remove(entity);
            _repository.SaveChanges();
        }
    }
}
