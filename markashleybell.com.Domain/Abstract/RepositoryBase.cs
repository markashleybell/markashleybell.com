using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using markashleybell.com.Domain.Concrete;
using System.Data.Entity;

namespace markashleybell.com.Domain.Abstract
{
    public abstract class RepositoryBase<T> where T : class
    {
        private Db _database;
        private readonly IDbSet<T> _dbset;

        protected RepositoryBase(IDbFactory databaseFactory)
        {
            DatabaseFactory = databaseFactory;
            _dbset = Db.Set<T>();
        }

        protected IDbFactory DatabaseFactory
        {
            get;
            private set;
        }

        protected Db Db
        {
            get { return _database ?? (_database = DatabaseFactory.Get()); }
        }

        public virtual void Add(T entity)
        {
            _dbset.Add(entity);
        }

        public virtual void Delete(T entity)
        {
            _dbset.Remove(entity);
        }

        public virtual T GetById(long id)
        {
            return _dbset.Find(id);
        }

        public virtual IEnumerable<T> All()
        {
            return _dbset.ToList();
        }
    }
}
