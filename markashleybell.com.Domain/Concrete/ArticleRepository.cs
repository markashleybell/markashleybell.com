using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using markashleybell.com.Domain.Abstract;
using markashleybell.com.Domain.Entities;

namespace markashleybell.com.Domain.Concrete
{
    public class ArticleRepository : RepositoryBase<Article>, IArticleRepository
    {
        public ArticleRepository(IDbFactory databaseFactory) : base(databaseFactory) { }

        public IEnumerable<Article> GetAll()
        {
            return base.Db.Articles.ToList();
        }

        public Article GetById(int id)
        {
            return base.Db.Articles.Where(x => x.ArticleID == id).FirstOrDefault();
        }

        public Article GetByUrl(string url)
        {
            return base.Db.Articles.Where(x => x.Slug == url).FirstOrDefault();
        }

        public int Save(Article article)
        {
            base.Db.Articles.Add(article);
            base.Db.Commit();

            return article.ArticleID;
        }

        public int Delete(Article article)
        {
            // int id = 

            base.Db.Articles.Remove(article);
            base.Db.Commit();

            return article.ArticleID;
        }
    }
}
