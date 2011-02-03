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
    public class AdminController : BaseController
    {
        public AdminController(IUnitOfWork unitOfWork, IArticleRepository articleRepository, ICommentRepository commentRepository) : base(unitOfWork, articleRepository, commentRepository) { }

        public ActionResult Index()
        {
            var articles = _articleRepository.All().Map();

            return View(articles);
        }

        public ActionResult Create()
        {
            return View();
        } 

        [HttpPost]
        public ActionResult Create(ArticleViewModel model)
        {
            model.Published = (model.Published != null && model.Published != DateTime.MinValue) ? model.Published : DateTime.Now;
            model.Updated = model.Published;

            var article = model.Map();

            _articleRepository.Add(article);

            _unitOfWork.Commit();

            return RedirectToAction("Edit", new { id = article.ArticleID });
        }
 
        public ActionResult Edit(int id)
        {
            var article = _articleRepository.Get(id).Map();

            return View(article);
        }

        [HttpPost]
        public ActionResult Edit(ArticleViewModel model)
        {
            var article = _articleRepository.Get(model.ArticleID);

            article.MapFrom(model);

            _unitOfWork.Commit();

            return View(model);
        }
        
        [HttpPost]
        public ActionResult Delete(int id)
        {
            _articleRepository.Remove(id);

            _unitOfWork.Commit();

            return RedirectToAction("");
        }
    }
}
