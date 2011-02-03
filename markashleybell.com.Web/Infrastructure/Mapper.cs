using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using markashleybell.com.Web.Models;
using markashleybell.com.Domain.Entities;
using MarkdownSharp;

namespace markashleybell.com.Web.Infrastructure
{
    public static class Mapper
    {
        public static void MapFrom(this ArticleViewModel model, Article article)
        {
            var md = new Markdown();

            model.Title = article.Title;
            model.Author = article.Author;
            model.Summary = article.Summary;
            model.SummaryHtml = article.SummaryHtml;
            model.Body = article.Body;
            model.BodyHtml = article.BodyHtml;
            model.Slug = article.Slug;
            model.Published = article.Published;
            model.Updated = article.Updated;

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
                        BodyHtml = md.Transform(comment.Body),
                        Published = comment.Published,
                        Updated = comment.Updated
                    });
                }
            }

        }

        public static void MapFrom(this Article article, ArticleViewModel model)
        {
            var md = new Markdown();

            article.Title = model.Title;
            article.Author = model.Author;
            article.Summary = model.Summary;
            article.SummaryHtml = md.Transform(model.Summary);
            article.Body = model.Body;
            article.BodyHtml = md.Transform(model.Body);
            article.Slug = model.Slug;
            article.Published = model.Published;
            article.Updated = DateTime.Now;
        }

        public static Article Map(this ArticleViewModel article)
        {
            var md = new Markdown();

            var model = new Article
            {
                ArticleID = article.ArticleID,
                Title = article.Title,
                Author = article.Author,
                Summary = article.Summary,
                SummaryHtml = md.Transform(article.Summary),
                Body = article.Body,
                BodyHtml = md.Transform(article.Body),
                Slug = article.Slug,
                Published = article.Published,
                Updated = article.Updated
            };

            if (article.Comments != null)
            {
                foreach (CommentViewModel comment in article.Comments)
                {
                    model.Comments.Add(new Comment
                    {
                        CommentID = comment.CommentID,
                        AuthorName = comment.AuthorName,
                        Email = comment.Email,
                        Url = comment.Url,
                        Body = comment.Body,
                        BodyHtml = md.Transform(comment.Body),
                        Published = comment.Published,
                        Updated = comment.Updated
                    });
                }
            }

            return model;
        }

        public static ArticleViewModel Map(this Article article)
        {
            var model = new ArticleViewModel {
                ArticleID = article.ArticleID,
                Title = article.Title,
                Author = article.Author,
                Summary = article.Summary,
                SummaryHtml = article.SummaryHtml,
                Body = article.Body,
                BodyHtml = article.BodyHtml,
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
                        BodyHtml = comment.BodyHtml,
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