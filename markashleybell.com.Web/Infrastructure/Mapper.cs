using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using markashleybell.com.Web.Models;
using markashleybell.com.Domain.Entities;

namespace markashleybell.com.Web.Infrastructure
{
    public static class Mapper
    {
        public static ArticleViewModel Map(this Article article)
        {
            var model = new ArticleViewModel {
                ArticleID = article.ArticleID,
                Title = article.Title,
                Author = article.Author,
                Summary = article.Summary,
                SummaryMarkdown = article.SummaryMarkdown,
                Body = article.Body,
                BodyMarkdown = article.BodyMarkdown,
                Slug = article.Slug,
                Published = article.Published,
                Updated = article.Updated
            };

            if (article.Comments != null)
            {
                foreach (Comment comment in article.Comments)
                {
                    model.Comments.Add(new CommentViewModel
                    {
                        CommentID = comment.CommentID,
                        AuthorName = comment.AuthorName,
                        Email = comment.Email,
                        Url = comment.Url,
                        Body = comment.Body,
                        BodyMarkdown = comment.BodyMarkdown,
                        Published = comment.Published,
                        Updated = comment.Updated
                    });
                }
            }

            return model;
        }

        public static List<ArticleViewModel> Map(this IEnumerable<Article> articles)
        {
            var model = new List<ArticleViewModel>();
            foreach (Article article in articles)
                model.Add(article.Map());
            return model;
        }
    }
}