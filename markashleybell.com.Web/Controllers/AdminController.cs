using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using markashleybell.com.Web.Infrastructure;
using markashleybell.com.Domain.Abstract;
using markashleybell.com.Domain.Entities;
using markashleybell.com.Web.Models;
using AutoMapper;
using MarkdownSharp;
using System.Text.RegularExpressions;

namespace markashleybell.com.Web.Controllers
{
    [Authorize]
    public class AdminController : BaseController
    {
        private Markdown _md;

        public AdminController(IUnitOfWork unitOfWork, IArticleRepository articleRepository, ICommentRepository commentRepository) : base(unitOfWork, articleRepository, commentRepository) 
        {
            _md = new Markdown();
        }

        public ActionResult Index()
        {
            return RedirectToAction("Articles");
        }

        public ActionResult Articles()
        {
            var articles = Mapper.Map<IEnumerable<Article>, IEnumerable<ArticleViewModel>>(_articleRepository.All());

            return View(articles);
        }

        public ActionResult CreateArticle()
        {
            return View();
        } 

        [HttpPost]
        public ActionResult CreateArticle(ArticleViewModel model)
        {
            model.Published = (model.Published != null && model.Published != DateTime.MinValue) ? model.Published : DateTime.Now;
            model.Updated = model.Published;

            model.Slug = (string.IsNullOrEmpty(model.Slug)) ? GetSlug(model.Title) : model.Slug;
            model.SummaryHtml = _md.Transform(model.Summary);
            model.BodyHtml = _md.Transform(model.Body);

            var article = Mapper.Map<ArticleViewModel, Article>(model);

            _articleRepository.Add(article);

            _unitOfWork.Commit();

            return RedirectToAction("EditArticle", new { id = article.ArticleID });
        }
 
        public ActionResult EditArticle(int id)
        {
            var article = Mapper.Map<Article, ArticleViewModel>(_articleRepository.Get(id));

            return View(article);
        }

        [HttpPost]
        public ActionResult EditArticle(ArticleViewModel model)
        {
            var article = _articleRepository.Get(model.ArticleID);

            article.Slug = (string.IsNullOrEmpty(model.Slug)) ? GetSlug(model.Title) : model.Slug;
            article.Author = model.Author;

            article.Published = (model.Published != null && model.Published != DateTime.MinValue) ? model.Published : DateTime.Now;
            article.Updated = DateTime.Now;

            article.Title = model.Title;
            article.Summary = model.Summary;
            article.SummaryHtml = _md.Transform(model.Summary);
            article.Body = model.Body;
            article.BodyHtml = _md.Transform(model.Body);

            _unitOfWork.Commit();

            return View(model);
        }
        
        [HttpPost]
        public ActionResult DeleteArticle(int id)
        {
            _articleRepository.Remove(id);

            _unitOfWork.Commit();

            return RedirectToAction("");
        }

        private static string GetSlug(string input)
        {
            RegexOptions o = RegexOptions.IgnoreCase | RegexOptions.Singleline;

            // Remove all special chars (but not spaces or dashes)
            string output = Regex.Replace(input, @"[^a-z0-9\s\-]", "", o);

            // Replace spaces with hyphens
            output = Regex.Replace(output, @"[\s]", "-", o);

            // Replace multiple hyphens (more than one in a row) with a single hyphen
            output = Regex.Replace(output, @"\-{2,}", "-", o);

            // Trim the extra hyphen off the end if exists

            return output.Trim(new char[] { '-', ' ' }).ToLower();
        }
    }
}
