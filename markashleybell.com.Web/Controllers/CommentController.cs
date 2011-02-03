using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using markashleybell.com.Domain.Abstract;
using markashleybell.com.Web.Models;
using markashleybell.com.Domain.Entities;

namespace markashleybell.com.Web.Controllers
{
    public class CommentController : BaseController
    {
        public CommentController(IUnitOfWork unitOfWork, IArticleRepository articleRepository, ICommentRepository commentRepository) : base(unitOfWork, articleRepository, commentRepository) { }

        [HttpPost]
        public ActionResult Add(CommentViewModel model)
        {
            var article = _articleRepository.Get(model.ArticleID);

            var now = DateTime.Now;

            _commentRepository.Add(new Comment
            {
                Article = article,
                AuthorName = model.AuthorName,
                Email = model.Email,
                Url = model.Url,
                Body = model.Body,
                BodyHtml = model.Body,
                Published = now,
                Updated = now
            });

            _unitOfWork.Commit();

            return Redirect("/articles/" + article.Slug);
        }
    }
}
