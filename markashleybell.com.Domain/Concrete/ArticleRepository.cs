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
        public ArticleRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public Article Get(int id)
        {
            return base.Get(id);
        }

        public Article GetByUrl(string url)
        {
            return base.Query(x => x.Slug == url).FirstOrDefault();
        }
    }
}
