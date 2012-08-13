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
    public class OAuthTokenCredentials 
    {
        public string Token { get; set; }
        public string TokenSecret { get; set; }
    }

    public class MainController : Controller
    {
        private string _consumerKey;
        private string _consumerSecret;
        private string _token;
        private string _tokenSecret;

        private OAuthTokenCredentials TokenCredentials
        {
            get { return Session["AccessCredentials"] as OAuthTokenCredentials; }
            set { Session["AccessCredentials"] = value; }
        }

        public MainController()
        {
            _consumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
            _consumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
            _token = ConfigurationManager.AppSettings["Token"];
            _tokenSecret = ConfigurationManager.AppSettings["TokenSecret"];
        }

        public ActionResult Index()
        {
            var client = new RestClient();

            client.Authenticator = OAuth1Authenticator.ForProtectedResource(
                _consumerKey, _consumerSecret, _token, _tokenSecret
            );

            var request = new RestRequest("https://api.dropbox.com/1/account/info");

            var response = client.Execute(request);

            var request2 = new RestRequest("https://api.dropbox.com/1/metadata/sandbox");
            
            var response2 = client.Execute<DropboxFolder>(request2);

            var request3 = new RestRequest("https://api.dropbox.com/1/metadata/sandbox");
            request3.AddParameter("hash", response2.Data.hash);

            var response3 = client.Execute<DropboxFolder>(request3);

            if (response3.StatusCode == HttpStatusCode.NotModified)
                return Json(new { status = "Not Modified" }, JsonRequestBehavior.AllowGet);

            return Json(response3.Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Auth(bool? callback)
        {
            if(!callback.HasValue)
            {
                var client = new RestClient();

                client.Authenticator = OAuth1Authenticator.ForRequestToken(_consumerKey, _consumerSecret);
                var request = new RestRequest("https://api.dropbox.com/1/oauth/request_token", Method.POST);
                var response = client.Execute(request);

                var qs = HttpUtility.ParseQueryString(response.Content);
                
                TokenCredentials = new OAuthTokenCredentials { 
                    Token = qs["oauth_token"],
                    TokenSecret = qs["oauth_token_secret"]
                };

                request = new RestRequest("https://api.dropbox.com/1/oauth/authorize");
                request.AddParameter("oauth_token", TokenCredentials.Token);
                request.AddParameter("oauth_callback", Request.Url.ToString() + "?callback=true");

                var url = client.BuildUri(request).ToString();

                return Redirect(url);
            }
            else
            {
                var client = new RestClient();
                
                client.Authenticator = OAuth1Authenticator.ForAccessToken(
                    _consumerKey, _consumerSecret, TokenCredentials.Token, TokenCredentials.TokenSecret
                );

                var request = new RestRequest("https://api.dropbox.com/1/oauth/access_token", Method.POST);
                var response = client.Execute(request);

                var qs = HttpUtility.ParseQueryString(response.Content);
                TokenCredentials.Token = qs["oauth_token"];
                TokenCredentials.TokenSecret = qs["oauth_token_secret"];

                return Json(TokenCredentials, JsonRequestBehavior.AllowGet);
            }
        }
    }

    
}
