using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using System.ComponentModel;
using System.Configuration;
using RestSharp;
using RestSharp.Authenticators;
using markashleybell.com.Models;
using System.Text.RegularExpressions;
using MarkdownSharp;
using System.Web.Mvc.Razor;
using System.IO;

namespace markashleybell.com.Controllers
{
    [Authorize]
    public class ArticleController : Controller
    {
        private DropboxApi _api;
        private HttpContextBase _context;

        public ArticleController(HttpContextBase context)
        {
            _api = new DropboxApi(ConfigurationManager.AppSettings["ConsumerKey"],
                                  ConfigurationManager.AppSettings["ConsumerSecret"],
                                  ConfigurationManager.AppSettings["Token"], 
                                  ConfigurationManager.AppSettings["TokenSecret"]);
            _context = context;
        }

        public ActionResult Index()
        {
            var fileList = _api.GetFileList("/articles", null);

            var slugs = (from file in fileList.contents
                         select new ArticleViewModel {
                             Slug = Path.GetFileNameWithoutExtension(file.path)
                         }).ToList();

            return View(new ArticleIndexViewModel {
                Articles = slugs
            });
        }

        private ArticleViewModel GenerateArticle(string slug)
        {
            var model = new ArticleViewModel();

            var rawContent = _api.GetFileContent("/articles/" + slug + ".md");

            model.Title = Regex.Match(rawContent, "^Title:\\s?(.*?)[\\r\\n]+", RegexOptions.Multiline).Groups[1].Value;
            var dateString = Regex.Match(rawContent, "^Date:\\s?(.*?)[\\r\\n]+", RegexOptions.Multiline).Groups[1].Value;
            model.PublishDate = DateTime.ParseExact(dateString, "yyyy-MM-dd HH:mm", null);

            rawContent = Regex.Replace(rawContent, "(^(?:Title|Date):\\s?.*?[\\r\\n]+)", "", RegexOptions.Multiline);

            // Retrieve all local images referenced in the document and store them on the server
            foreach(Match match in Regex.Matches(rawContent, "\\/content\\/img\\/(.*\\.gif|\\.jpg)"))
            {
                var fileName = match.Groups[1].Value;
                _api.DownloadFile("/img/" + fileName, Server.MapPath("~/Content/Img") + "/" + fileName);
            }

            model.Body = new Markdown().Transform(rawContent);
            model.Slug = slug;

            return model;
        }

        private ArticleIndexViewModel GenerateArticleIndex()
        {
            // Generate a model for the article index
            var articles = new List<ArticleViewModel>();

            foreach(var file in Directory.GetFiles(Server.MapPath("~/Rendered"), "*.html"))
            {
                var slug = Path.GetFileNameWithoutExtension(file);

                if(slug != "index")
                {
                    // Only read the first 50 lines of each file - this should cover the HEAD section of the HTML
                    // TODO: Cope with files of less than 50 lines (unlikely, but still)
                    using(StreamReader reader = new StreamReader(file))
                    {
                        var lines = new string[50];
                        for(var i = 0; i < lines.Length; i++)
                        {
                            lines[i] = reader.ReadLine();
                        }

                        var html = string.Join("", lines);

                        var publishDate = Regex.Match(html, "<meta name=\"publishdate\" content=\"(.*?)\"\\s?/?>").Groups[1].Value;

                        var article = new ArticleViewModel {
                            Title = Regex.Match(html, "<title>(.*)</title>").Groups[1].Value,
                            PublishDate = (publishDate == "") ? DateTime.MinValue : DateTime.ParseExact(publishDate, "yyyy-MM-dd HH:mm", null),
                            Slug = slug
                        };

                        articles.Add(article);
                    }
                }
            }

            // Sort articles by descending date
            var indexModel = new ArticleIndexViewModel {
                Articles = articles.OrderByDescending(x => x.PublishDate).ToList()
            };

            return indexModel;
        }

        private void RenderStaticView(object model, string view, string layout, string outputPath)
        {
            // Render a static HTML file using our Razor views - this is what users will be directed to when they are viewing the page normally
            using(var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindView(ControllerContext, view, layout);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, new ViewDataDictionary(model), new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);
                System.IO.File.WriteAllText(outputPath, sw.GetStringBuilder().ToString());
            }
        }

        public ActionResult RenderAndPreview(string slug)
        {
            var model = GenerateArticle(slug);

            // Render the article view
            RenderStaticView(model, "RenderAndPreview", "MainLayout", Server.MapPath("~/Rendered/" + slug + ".html"));

            var indexModel = GenerateArticleIndex();

            // Render the index view
            RenderStaticView(indexModel, "ArticleIndex", "MainLayout", Server.MapPath("~/Rendered/index.html"));

            return View(model);
        }

        public ActionResult Rebuild()
        {
            // Delete all files from rendered folder to purge any that no longer exist on Dropbox
            foreach(var file in Directory.GetFiles(Server.MapPath("~/Rendered"), "*.html"))
                System.IO.File.Delete(file);

            var fileList = _api.GetFileList("/articles", null);

            var slugs = (from file in fileList.contents
                         select Path.GetFileNameWithoutExtension(file.path)).ToList();

            foreach(var slug in slugs)
            {
                var model = GenerateArticle(slug);

                RenderStaticView(model, "RenderAndPreview", "MainLayout", Server.MapPath("~/Rendered/" + slug + ".html"));
            }

            var indexModel = GenerateArticleIndex();

            // Render the index
            RenderStaticView(indexModel, "ArticleIndex", "MainLayout", Server.MapPath("~/Rendered/index.html"));

            return Content("Done");
        }

        public ActionResult Auth(bool? callback)
        {
            if(!callback.HasValue)
            {
                _api.GetRequestToken();

                _context.Session["token"] = _api.Token;
                _context.Session["tokensecret"] = _api.TokenSecret;

                var redirectUrl = _api.GetAuthorizeUrl(Request.Url.ToString() + "?callback=true");

                return Redirect(redirectUrl);
            }
            else
            {
                _api.Token = _context.Session["token"].ToString();
                _api.TokenSecret = _context.Session["tokensecret"].ToString();

                _api.GetAccessToken();

                // Create subfolders within the app sandbox folder
                _api.CreateFolder("/img");
                _api.CreateFolder("/articles");

                return Json(new { Token = _api.Token, Secret = _api.TokenSecret }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult About()
        {
            var info = _api.GetAccountInfo();

            return Json(info, JsonRequestBehavior.AllowGet);
        }
    }
}
