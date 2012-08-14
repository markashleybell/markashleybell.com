using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;

namespace markashleybell.com.Models
{
    public class DropboxApi
    {
        private const string _standardApiBaseUrl = "https://api.dropbox.com/1";
        private const string _contentApiBaseUrl = "https://api-content.dropbox.com/1";

        private string _consumerKey;
        private string _consumerSecret;
        private string _token;
        private string _tokenSecret;

        public DropboxApi(string consumerKey, string consumerSecret, string token, string tokenSecret)
        {
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
            _token = token;
            _tokenSecret = tokenSecret;
        }

        public string Token
        {
            get { return _token; }
            set { _token = value; }
        }

        public string TokenSecret
        {
            get { return _tokenSecret; }
            set { _tokenSecret = value; }
        }

        public void GetRequestToken()
        {
            var client = new RestClient(_standardApiBaseUrl);

            client.Authenticator = OAuth1Authenticator.ForRequestToken(_consumerKey, _consumerSecret);

            var request = new RestRequest("/oauth/request_token", Method.POST);
            var response = client.Execute(request);

            var qs = HttpUtility.ParseQueryString(response.Content);

            _token = qs["oauth_token"];
            _tokenSecret = qs["oauth_token_secret"];
        }

        public string GetAuthorizeUrl(string callbackUrl)
        {
            var client = new RestClient(_standardApiBaseUrl);

            client.Authenticator = OAuth1Authenticator.ForRequestToken(_consumerKey, _consumerSecret);

            var request = new RestRequest("/oauth/authorize");
            request.AddParameter("oauth_token", _token);

            if(callbackUrl != null)
                request.AddParameter("oauth_callback", callbackUrl);

            var url = client.BuildUri(request).ToString();

            return url;
        }

        public void GetAccessToken()
        {
            var client = new RestClient(_standardApiBaseUrl);

            client.Authenticator = OAuth1Authenticator.ForAccessToken(
                _consumerKey, _consumerSecret, _token, _tokenSecret
            );

            var request = new RestRequest("/oauth/access_token", Method.POST);
            var response = client.Execute(request);

            var qs = HttpUtility.ParseQueryString(response.Content);

            _token = qs["oauth_token"];
            _tokenSecret = qs["oauth_token_secret"];
        }

        public DropboxAccountInfo GetAccountInfo()
        {
            var client = new RestClient(_standardApiBaseUrl);

            client.Authenticator = OAuth1Authenticator.ForProtectedResource(
                _consumerKey, _consumerSecret, _token, _tokenSecret
            );

            var request = new RestRequest("/account/info");

            var response = client.Execute<DropboxAccountInfo>(request);

            return response.Data;
        }

        public DropboxFolder GetFiles(string hash)
        {
            return GetFiles("", hash);
        }

        public DropboxFolder GetFiles(string path, string hash)
        {
            var client = new RestClient(_contentApiBaseUrl);

            client.Authenticator = OAuth1Authenticator.ForProtectedResource(
                _consumerKey, _consumerSecret, _token, _tokenSecret
            );

            var request = new RestRequest("/metadata/sandbox" + path);

            if(hash != null)
                request.AddParameter("hash", hash);

            var response = client.Execute<DropboxFolder>(request);

            if (response.StatusCode == HttpStatusCode.NotModified)
                return null;

            return response.Data;
        }
    }
}