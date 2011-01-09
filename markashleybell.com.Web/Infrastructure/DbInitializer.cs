using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Infrastructure;
using markashleybell.com.Domain.Concrete;
using markashleybell.com.Domain.Entities;

namespace markashleybell.com.Web.Infrastructure
{
    public class DbInitializer : RecreateDatabaseIfModelChanges<Db>
    {
        protected override void Seed(Db context)
        {
            var article = new Article
            {
                Title = "Test Article",
                Author = "Mark Bell",
                Summary = "This is a summary.",
                SummaryMarkdown = "This is a summary.",
                Body = "This is the test article text.",
                BodyMarkdown = "This is the test article text.",
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
                        BodyMarkdown = "This is a test",
                        Published = DateTime.Now,
                        Updated = DateTime.Now
                    },
                    new Comment 
                    {
                        AuthorName = "Jim Smith",
                        Email = "jim@jim.com",
                        Url = "",
                        Body = "Jim says hello!",
                        BodyMarkdown = "Jim says hello!",
                        Published = DateTime.Now,
                        Updated = DateTime.Now
                    }
                }
            };

            context.Articles.Add(article);
        }
    }
}