using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using markashleybell.com.Web.Infrastructure;
using markashleybell.com.Domain.Abstract;
using markashleybell.com.Domain.Entities;
using markashleybell.com.Web.Models;

namespace markashleybell.com.Web.Controllers
{
    public class ArticleController : BaseController
    {
        public ArticleController(IUnitOfWork unitOfWork, IArticleRepository articleRepository, ICommentRepository commentRepository) : base(unitOfWork, articleRepository, commentRepository) { }

        public ActionResult Index()
        {
            var articles = _articleRepository.All().Map();

            return View(articles);
        }

        [HttpGet]
        public ActionResult Article(string url)
        {
            var article = _articleRepository.GetByUrl(url);

            if (article == null)
                return Redirect("/pagenotfound");

            return View(article.Map());
        }
    }
}
