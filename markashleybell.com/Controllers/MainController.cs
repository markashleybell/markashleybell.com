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

namespace markashleybell.com.Controllers
{
    public class MainController : Controller
    {
        private DropboxApi _api;
        private HttpSessionStateBase _session;

        public MainController(HttpSessionStateBase session)
        {
            _api = new DropboxApi(ConfigurationManager.AppSettings["ConsumerKey"],
                                  ConfigurationManager.AppSettings["ConsumerSecret"],
                                  ConfigurationManager.AppSettings["Token"], 
                                  ConfigurationManager.AppSettings["TokenSecret"]);

            _session = session;
        }

        public ActionResult Index()
        {
            string hash = (_session["hash"] == null) ? null : _session["hash"].ToString();

            _api.Token = _session["token"].ToString();
            _api.TokenSecret = _session["tokensecret"].ToString();

            var folder = _api.GetFiles(hash);

            _session["hash"] = folder.hash;

            return Json(folder, JsonRequestBehavior.AllowGet);
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

                _session["token"] = _api.Token;
                _session["tokensecret"] = _api.TokenSecret;

                var redirectUrl = _api.GetAuthorizeUrl(Request.Url.ToString() + "?callback=true");

                return Redirect(redirectUrl);
            }
            else
            {
                _api.Token = _session["token"].ToString();
                _api.TokenSecret = _session["tokensecret"].ToString();

                _api.GetAccessToken();

                return Json(new { Token = _api.Token, Secret = _api.TokenSecret }, JsonRequestBehavior.AllowGet);
            }
        }
    }

    
}
