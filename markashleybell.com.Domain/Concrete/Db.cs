using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using markashleybell.com.Domain.Entities;

namespace markashleybell.com.Domain.Concrete
{
    public class Db : DbContext
    {
        private IDbSet<Article> _articles;
        private IDbSet<Comment> _comments;

        public IDbSet<Article> Articles
        {
            get { return _articles ?? (_articles = DbSet<Article>()); }
        }

        public IDbSet<Comment> Comments
        {
            get { return _comments ?? (_comments = DbSet<Comment>()); }
        }

        public virtual IDbSet<T> DbSet<T>() where T : class
        {
            return Set<T>();
        }

        public virtual void Commit()
        {
            base.SaveChanges();
        }
    }
}
