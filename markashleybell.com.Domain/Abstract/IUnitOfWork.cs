using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using markashleybell.com.Domain.Concrete;

namespace markashleybell.com.Domain.Abstract
{
    public interface IUnitOfWork
    {
        Db Database { get; }
        void Commit();
    }
}
