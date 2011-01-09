using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using markashleybell.com.Domain.Extensions;
using markashleybell.com.Domain.Abstract;

namespace markashleybell.com.Domain.Concrete
{
    public class DbFactory : Disposable, IDbFactory
    {
        private Db _database;

        public Db Get()
        {
            return _database ?? (_database = new Db());
        }

        protected override void DisposeCore()
        {
            if (_database != null) _database.Dispose();
        }
    }
}
