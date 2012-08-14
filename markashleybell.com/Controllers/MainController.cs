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

        public ActionResult Post(string slug)
        {
            var file = _api.GetFileUrl("/" + slug + ".md");

            var model = new PostViewModel();

            using(var get = new WebClient())
            {
                var rawContent = get.DownloadString(file.url);

                model.Title = Regex.Match(rawContent, "^Title:\\s?(.*?)[\\r\\n]+", RegexOptions.Multiline).Groups[1].Value;
                var dateString = Regex.Match(rawContent, "^Date:\\s?(.*?)[\\r\\n]+", RegexOptions.Multiline).Groups[1].Value;
                model.Date = DateTime.ParseExact(dateString, "yyyy-MM-dd hh:mm", null);

                rawContent = Regex.Replace(rawContent, "(^(?:Title|Date):\\s?.*?[\\r\\n]+)", "", RegexOptions.Multiline);

                model.Body = new Markdown().Transform(rawContent);
            }

            var template = System.IO.File.ReadAllText(Server.MapPath("~/Views/Shared/MainLayout.cshtml"));

            // Render a static HTML file - this is what users will be directed to when they are viewing the page
            using(var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindView(ControllerContext, "Post", "MainLayout");
                var viewContext = new ViewContext(ControllerContext, viewResult.View, new ViewDataDictionary(model), new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);
                System.IO.File.WriteAllText(Server.MapPath("~/Rendered/" + slug + ".html"), sw.GetStringBuilder().ToString());
            }

            // TODO: This will need to render the home page/archive at the same time
            // Top 5 posts in descending order
            // Name MD source files 2012-08-01-08-35-this-is-the-slug, that way ordering will be simple?
            // Archive can just be a massive list for now
            // Include abstract in header?

            return View(model);
        }

        public ActionResult AccountInfo()
        {
            var info = _api.GetAccountInfo();

            return Json(info, JsonRequestBehavior.AllowGet);
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

                return Json(new { Token = _api.Token, Secret = _api.TokenSecret }, JsonRequestBehavior.AllowGet);
            }
        }
    }

    
}
