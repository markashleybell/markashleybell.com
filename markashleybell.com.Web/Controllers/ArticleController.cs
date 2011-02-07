﻿using System;
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
using System.Net;

namespace markashleybell.com.Web.Controllers
{
    public class ArticleController : BaseController
    {
        private Markdown _md;

        public ArticleController(IUnitOfWork unitOfWork, IArticleRepository articleRepository, ICommentRepository commentRepository) : base(unitOfWork, articleRepository, commentRepository) 
        {
            _md = new Markdown();
        }

        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            var articles = Mapper.Map<IEnumerable<Article>, IEnumerable<ArticleViewModel>>(_articleRepository.All());

            return View(articles);
        }

        [HttpGet]
        public ActionResult Article(string url)
        {
            var article = _articleRepository.GetByUrl(url);

            if (article == null)
                throw new HttpException((int)HttpStatusCode.NotFound, "");

            var viewModel = Mapper.Map<Article, ArticleDetailPageViewModel>(article);

            viewModel.Comments.Sort(delegate(CommentViewModel a, CommentViewModel b) { return DateTime.Compare(a.Published, b.Published); });

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ValidateComment(ArticleDetailPageViewModel model)
        {
            var errors = (from item in ModelState
                          where item.Value.Errors.Any()
                          select new
                          {
                              field = item.Key[0].ToString().ToUpper() + item.Key.Substring(1),
                              error = (from e in item.Value.Errors
                                       select new { message = e.ErrorMessage }).ToList()
                          }).ToList();

            return Json(errors);
        }

        [HttpPost]
        public ActionResult Article(ArticleDetailPageViewModel model)
        {
            var article = _articleRepository.Get(model.ArticleID);

            if (article == null)
                return Redirect("/pagenotfound");

            var newComment = model.Comment;

            if (!ModelState.IsValid)
            {
                model = Mapper.Map<Article, ArticleDetailPageViewModel>(article);
                return View(model);
            }

            newComment.Published = DateTime.Now;
            newComment.Updated = newComment.Published;
            newComment.BodyHtml = _md.Transform(newComment.Body);

            var comment = Mapper.Map<CommentViewModel, Comment>(newComment);

            article.Comments.Add(comment);

            _unitOfWork.Commit();

            return Redirect("/articles/" + article.Slug);
        }
    }
}
