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

        [HttpPost]
        public ActionResult Article(ArticleViewModel model)
        {
            var article = _articleRepository.Get(model.ArticleID);

            if (!ModelState.IsValid)
            {
                var a = article.Map();

                model.Title = a.Title;
                model.Author = a.Author;
                model.BodyMarkdown = a.BodyMarkdown;
                model.Published = a.Published;
                model.Comments = a.Comments.ToList();

                return View(model);
            }

            var now = DateTime.Now;

            article.Comments.Add(new Comment {
                Article = article,
                AuthorName = model.NewComment.AuthorName,
                Email = model.NewComment.Email,
                Url = model.NewComment.Url,
                Body = model.NewComment.Body,
                BodyMarkdown = model.NewComment.Body,
                Published = now,
                Updated = now
            });

            _unitOfWork.Commit();

            return RedirectToAction("Article", new { url = model.Slug });
        }
    }
}
