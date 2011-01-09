using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markashleybell.com.Domain.Abstract
{
    public interface IRepository<T> where T : class
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        int Save(T entity);
        int Delete(T entity);
    }
}
