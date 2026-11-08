using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Revelation.Front.Office.Core.Interfaces;

namespace Revelation.Front.Office.Repositories.EF.Generic
{
    /// <summary>Default IRepository&lt;T&gt; implementation shared by every EF-backed repository.</summary>
    public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext Context;
        protected readonly DbSet<TEntity> Set;

        public EfRepository(DbContext context)
        {
            Context = context;
            Set = context.Set<TEntity>();
        }

        public IQueryable<TEntity> Query() => Set.AsQueryable();

        public TEntity GetById(int id) => Set.Find(id);

        public IEnumerable<TEntity> GetAll() => Set.ToList();

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate) => Set.Where(predicate).ToList();

        public TEntity Add(TEntity entity)
        {
            Set.Add(entity);
            return entity;
        }

        public void Update(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(TEntity entity)
        {
            Set.Remove(entity);
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }
    }
}
