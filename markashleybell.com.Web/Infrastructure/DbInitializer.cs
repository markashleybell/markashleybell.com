using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Infrastructure;
using markashleybell.com.Domain.Concrete;
using markashleybell.com.Domain.Entities;
using System.Data.Entity.Database;

namespace markashleybell.com.Web.Infrastructure
{
    public class DbInitializer : DropCreateDatabaseIfModelChanges<Db>
    {
        protected override void Seed(Db context)
        {
            var article = new Article
            {
                Title = "Test Article",
                Author = "Mark Bell",
                Summary = "This is a summary.",
                SummaryMarkdown = "<p>This is a summary.</p>",
                Body = "This is the test article text.",
                BodyMarkdown = "<p>This is the test article text.</p>",
                Slug = "test-article",
                Published = DateTime.Now,
                Updated = DateTime.Now,
                Comments = new List<Comment> 
                {
                    new Comment 
                    {
                        AuthorName = "Bob Jones",
                        Email = "bob@bob.com",
                        Url = "http://jim.com/",
                        Body = "This is a test",
                        BodyMarkdown = "<p>This is a test</p>",
                        Published = DateTime.Now,
                        Updated = DateTime.Now
                    },
                    new Comment 
                    {
                        AuthorName = "Jim Smith",
                        Email = "jim@jim.com",
                        Url = "",
                        Body = "Jim says hello!",
                        BodyMarkdown = "<p>Jim says hello!</p>",
                        Published = DateTime.Now,
                        Updated = DateTime.Now
                    }
                }
            };

            var article2 = new Article
            {
                Title = "Test Article 2",
                Author = "Mark Bell",
                Summary = "This is a summary.",
                SummaryMarkdown = "<p>This is a summary.</p>",
                Body = "This is the test article 2 text.",
                BodyMarkdown = "<p>This is the test article 2 text.</p>",
                Slug = "test-article-2",
                Published = DateTime.Now,
                Updated = DateTime.Now,
                Comments = new List<Comment> 
                {
                    new Comment 
                    {
                        AuthorName = "Jim Smith",
                        Email = "jim@jim.com",
                        Url = "",
                        Body = "Jim says hello!",
                        BodyMarkdown = "<p>Jim says hello!</p>",
                        Published = DateTime.Now,
                        Updated = DateTime.Now
                    },
                    new Comment 
                    {
                        AuthorName = "Bob Jones",
                        Email = "bob@bob.com",
                        Url = "http://jim.com/",
                        Body = "This is a test",
                        BodyMarkdown = "<p>This is a test</p>",
                        Published = DateTime.Now,
                        Updated = DateTime.Now
                    }
                }
            };

            context.Articles.Add(article);
            context.Articles.Add(article2);
        }
    }
}