using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Revelation.Front.Office.Core.Interfaces
{
    /// <summary>
    /// Generic data-access contract implemented by Repositories for every
    /// entity type. Kept intentionally small (no Unit-of-Work, no spec
    /// pattern) since this phase is about establishing the layering, not
    /// building out a full persistence framework.
    /// </summary>
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Query();
        TEntity GetById(int id);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        TEntity Add(TEntity entity);
        void Update(TEntity entity);
        void Remove(TEntity entity);
        void SaveChanges();
    }
}
