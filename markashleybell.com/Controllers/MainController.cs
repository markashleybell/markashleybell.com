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

namespace markashleybell.com.Controllers
{
    public class MainController : Controller
    {
        private DropboxApi _api;
        private HttpSessionStateBase _sessionState;
        private HttpApplicationStateBase _applicationState;

        public MainController(HttpApplicationStateBase applicationState, HttpSessionStateBase sessionState)
        {
            _api = new DropboxApi(ConfigurationManager.AppSettings["ConsumerKey"],
                                  ConfigurationManager.AppSettings["ConsumerSecret"],
                                  ConfigurationManager.AppSettings["Token"], 
                                  ConfigurationManager.AppSettings["TokenSecret"]);

            _sessionState = sessionState;
            _applicationState = applicationState;
        }

        public ActionResult Index()
        {
            string hash = (_sessionState["hash"] == null) ? null : _sessionState["hash"].ToString();

            var folder = _api.GetFileList(hash);

            if(folder != null)
                _sessionState["hash"] = folder.hash;

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

                _sessionState["token"] = _api.Token;
                _sessionState["tokensecret"] = _api.TokenSecret;

                var redirectUrl = _api.GetAuthorizeUrl(Request.Url.ToString() + "?callback=true");

                return Redirect(redirectUrl);
            }
            else
            {
                _api.Token = _sessionState["token"].ToString();
                _api.TokenSecret = _sessionState["tokensecret"].ToString();

                _api.GetAccessToken();

                return Json(new { Token = _api.Token, Secret = _api.TokenSecret }, JsonRequestBehavior.AllowGet);
            }
        }
    }

    
}
