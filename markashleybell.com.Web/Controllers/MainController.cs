using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using markashleybell.com.Domain.Abstract;
using System.Net;

namespace markashleybell.com.Web.Controllers
{
    public class MainController : BaseController
    {
        public MainController(IUnitOfWork unitOfWork, IArticleRepository articleRepository, ICommentRepository commentRepository) : base(unitOfWork, articleRepository, commentRepository) { }

        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            return View();
        }

        [OutputCache(Duration = 3600)]
        public ActionResult About()
        {
            return View();
        }

        [OutputCache(Duration = 3600)]
        public ActionResult JQuery()
        {
            return View();
        }

        public ActionResult NotFoundRedirect()
        {
            throw new HttpException((int)HttpStatusCode.NotFound, "");
        }

        [OutputCache(Duration = 3600)]
        public ActionResult SiteMapXml()
        {
            var urlList = new List<string>();
            var baseUrl = "http://markashleybell.com";

            urlList.AddRange(new string[] {
                baseUrl,
                baseUrl + "/about",
                baseUrl + "/articles",
                baseUrl + "/jquery",
                baseUrl + "/jquery/jquery.nesteddecimallist.html",
                baseUrl + "/jquery/jquery.caption.html",
                baseUrl + "/jquery/jquery.externallink.html",
                baseUrl + "/jquery/jquery.popmenu.html", 
                baseUrl + "/jquery/jquery.listselect.html" 
            });

            var articles = _articleRepository.All().ToList();

            foreach (var article in articles)
                urlList.Add(baseUrl + "/articles/" + article.Slug);

            return View(urlList);
        }
    }
}
