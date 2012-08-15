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
    public class MainController : Controller
    {
        private DropboxApi _api;
        private HttpContextBase _context;

        public MainController(HttpContextBase context)
        {
            _api = new DropboxApi(ConfigurationManager.AppSettings["ConsumerKey"],
                                  ConfigurationManager.AppSettings["ConsumerSecret"],
                                  ConfigurationManager.AppSettings["Token"], 
                                  ConfigurationManager.AppSettings["TokenSecret"]);
            _context = context;
        }

        public ActionResult Index()
        {
            string hash = (_context.Session["hash"] == null) ? null : _context.Session["hash"].ToString();

            var folder = _api.GetFileList(hash);

            if(folder != null)
                _context.Session["hash"] = folder.hash;

            return Json(folder, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateAndPreview(string slug)
        {
            var model = new ArticleViewModel();

            var rawContent = _api.GetFileContent("/articles/" + slug + ".md");

            model.Title = Regex.Match(rawContent, "^Title:\\s?(.*?)[\\r\\n]+", RegexOptions.Multiline).Groups[1].Value;
            var dateString = Regex.Match(rawContent, "^Date:\\s?(.*?)[\\r\\n]+", RegexOptions.Multiline).Groups[1].Value;
            model.PublishDate = DateTime.ParseExact(dateString, "yyyy-MM-dd hh:mm", null);

            rawContent = Regex.Replace(rawContent, "(^(?:Title|Date):\\s?.*?[\\r\\n]+)", "", RegexOptions.Multiline);

            // Retrieve all local images referenced in the document and store them on the server
            foreach (Match match in Regex.Matches(rawContent, "\\/content\\/img\\/(.*\\.gif|\\.jpg)"))
            {
                var fileName = match.Groups[1].Value;
                _api.DownloadFile("/img/" + fileName, Server.MapPath("~/Content/Img") + "/" + fileName);
            }

            model.Body = new Markdown().Transform(rawContent);

            // Render a static HTML file using our Razor views - this is what users will be directed to when they are viewing the page normally
            using(var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindView(ControllerContext, "UpdateAndPreview", "MainLayout");
                var viewContext = new ViewContext(ControllerContext, viewResult.View, new ViewDataDictionary(model), new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);
                System.IO.File.WriteAllText(Server.MapPath("~/Rendered/" + slug + ".html"), sw.GetStringBuilder().ToString());
            }

            // This will get us a nice list of file paths keyed by date (and can be limited using the API), but will be
            // very slow to build the archive once there are a few hundred articles...
            // var folder = _api.GetFileList(null);

            //var fileList = (from f in folder.contents
            //                let info = Path.GetFileNameWithoutExtension(f.path)
            //                let date = Regex.Match(_api.GetFileContent(f.path), "^Date:\\s?(.*?)[\\r\\n]+", RegexOptions.Multiline).Groups[1].Value
            //                select new KeyValuePair<DateTime, string>(DateTime.ParseExact(date, "yyyy-MM-dd hh:mm", null), info)).ToList();

            // If we store the date and abstract as meta tags in the generated HTML for each page, we can use filesystem access 
            // to build the home page and archive, which will be massively faster

            // <title>(.*)</title>
            // <meta name="publishdate" content="(.*)"\s?/?>

            var articles = new List<ArticleViewModel>();

            foreach(var file in Directory.GetFiles(Server.MapPath("~/Rendered")))
            {
                // Only read the first 50 lines - this should cover the HEAD section of the HTML
                // TODO: Cope with files of less than 50 lines (unlikely, but still)
                using (StreamReader reader = new StreamReader(file))
                {
                    var lines = new string[50];
                    for (var i = 0; i < lines.Length; i++)
                    {
                        lines[i] = reader.ReadLine();
                    }

                    var html = string.Join("", lines);

                    var publishDate = Regex.Match(html, "<meta name=\"publishdate\" content=\"(.*?)\"\\s?/?>").Groups[1].Value;

                    var article = new ArticleViewModel {
                        Title = Regex.Match(html, "<title>(.*)</title>").Groups[1].Value,
                        PublishDate = (publishDate == "") ? DateTime.MinValue : DateTime.ParseExact(publishDate, "yyyy-MM-dd hh:mm", null)
                    };

                    articles.Add(article);
                }
            }

            var indexModel = new ArticleIndexViewModel {
                Articles = articles.OrderByDescending(x => x.PublishDate).ToList()
            };

            // Render a static HTML file using our Razor views - this is what users will be directed to when they are viewing the page normally
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindView(ControllerContext, "ArticleIndex", "MainLayout");
                var viewContext = new ViewContext(ControllerContext, viewResult.View, new ViewDataDictionary(indexModel), new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);
                System.IO.File.WriteAllText(Server.MapPath("~/Rendered/index.html"), sw.GetStringBuilder().ToString());
            }

            // We're going to need some kind of 'regenerate all' command too, for init and reset

            return View(model);
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
