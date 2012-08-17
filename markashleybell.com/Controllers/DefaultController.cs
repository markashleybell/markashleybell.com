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

        [HttpPost]
        [OutputCache(Duration = 1800)]
        public ActionResult RecentTwitterStatuses(int? count)
        {
            if (!count.HasValue)
                throw new HttpException((int)HttpStatusCode.NotFound, "Page Not Found");

            var request = new RestRequest("https://api.twitter.com/1/statuses/user_timeline.json?screen_name=markashleybell&count=" + count.Value);

            var response = new RestClient().Execute<List<Tweet>>(request);

            return Json(response.Data.Select(x => new {
                            date = x.created_at.ToPrettyDate("ddd MMM dd HH:mm:ss %zzzz yyyy"),
                            status = x.text
                        }));
        }

        //[HttpPost]
        //[OutputCache(Duration = 1800)]
        public ActionResult RecentGitHubActivity(int? count)
        {
            if (!count.HasValue)
                throw new HttpException((int)HttpStatusCode.NotFound, "Page Not Found");

            var request = new RestRequest("https://api.github.com/users/markashleybell/events/public");

            var response = new RestClient().Execute<List<RootObject>>(request);

            return Json(response.Data.Take(count.Value).Select(x => new {
                date = x.created_at.ToPrettyDate("yyyy-MM-ddTHH:mm:ssZ"),
                repo = x.repo.name,
                status = "TBD"
            }) , JsonRequestBehavior.AllowGet);
        }

        
    }
}
