using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using markashleybell.com.Web.Infrastructure;
using markashleybell.com.Domain.Abstract;
using markashleybell.com.Domain.Entities;

namespace markashleybell.com.Web.Controllers
{
    public class ArticleController : BaseController
    {
        public ArticleController(IArticleRepository articleRepository) : base(articleRepository) { }

        public ActionResult Index()
        {
            var articles = _articleRepository.GetAll().Map();



            return View(articles);
        }

        public ActionResult Article(string url)
        {
            return View();
        }
    }
}
