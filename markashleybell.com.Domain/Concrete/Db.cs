using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using markashleybell.com.Domain.Entities;
using System.Data.Entity.ModelConfiguration;

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

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.Edm.Db.ColumnTypeCasingConvention>();

        //    modelBuilder.Entity<Article>()
        //                .Property(p => p.Body)
        //                .HasColumnType("varchar(max)");
        //}
    }
}
