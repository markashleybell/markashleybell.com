using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using markashleybell.com.Domain.Abstract;

namespace markashleybell.com.Domain.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbFactory _databaseFactory;
        private Db _database;

        public UnitOfWork(IDbFactory databaseFactory)
        {
            _databaseFactory = databaseFactory;
        }

        public Db Database
        {
            get { return _database ?? (_database = _databaseFactory.Get()); }
        }

        public void Commit()
        {
            Database.Commit();
        }
    }
}
