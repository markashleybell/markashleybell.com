using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using RestSharp;
using markashleybell.com.Models;
using markashleybell.com.Extensions;

namespace markashleybell.com.Controllers
{
    public class DefaultController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RecentTwitterStatuses(int? count)
        {
            if (!count.HasValue)
                throw new HttpException((int)HttpStatusCode.NotFound, "Page Not Found");

            var client = new RestClient();
            var request = new RestRequest("https://api.twitter.com/1/statuses/user_timeline.json?screen_name=markashleybell&count=" + count.Value);

            var response = client.Execute<List<Tweet>>(request);

            return Json((response.Data.Select(x => new {
                             date = x.created_at.ToPrettyDate(),
                             status = x.text
                         })), JsonRequestBehavior.AllowGet);
        }
    }
}
