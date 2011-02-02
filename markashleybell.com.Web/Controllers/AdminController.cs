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
            var now = DateTime.Now;

            model.Published = now;
            model.Updated = now;

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

        //
        // POST: /Admin/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ArticleViewModel model)
        {
            var article = _articleRepository.Get(model.ArticleID);

            article.Title = model.Title;

            _unitOfWork.Commit();

            return View(model);
        }

        //
        // GET: /Admin/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Admin/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
