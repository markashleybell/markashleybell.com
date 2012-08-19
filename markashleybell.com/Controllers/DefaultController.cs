using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using RestSharp;
using markashleybell.com.Models;
using markashleybell.com.Extensions;
using System.Globalization;

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

            var response = new RestClient().Execute<List<Activity>>(request);

            var items = new List<ActivitySummary>();

            foreach (var item in response.Data)
            {
                var activity = new ActivitySummary {
                    date = item.created_at.ToPrettyDate("yyyy-MM-ddTHH:mm:ssZ"),
                };

                switch (item.type)
                {
                    case "WatchEvent":
                        activity.status = "Watched <a href=\"http://github.com/" + item.repo.name + "\">" + item.repo.name + "</a>";
                        break;
                    case "PushEvent":
                        var branch = item.payload.@ref.Substring(item.payload.@ref.LastIndexOf("/") + 1);
                        activity.status = "Pushed to " + branch + " branch on <a href=\"http://github.com/" + item.repo.name + "\">" + item.repo.name + "</a> ";
                        foreach (var commit in item.payload.commits)
                        {
                            activity.status += "<span><a href=\"http://github.com/" + item.repo.name + "/commit/" + commit.sha + "\">" + commit.sha.Substring(0, 6) + "</a>: " + commit.message + "</span> ";
                        }
                        break;
                    case "IssuesEvent":
                        activity.status = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.payload.action) + " <a href=\"" + item.payload.issue.html_url + "\">issue " + item.payload.issue.number + "</a> on <a href=\"http://github.com/" + item.repo.name + "\">" + item.repo.name + "</a><br />";
                        break;
                    case "IssueCommentEvent":
                        // activity.status = "COMMENT";
                        // Hide for now
                        break;
                    default:
                        break;
                }

                items.Add(activity);
            }

            return Json(items.Take(count.Value), JsonRequestBehavior.AllowGet);
        }

        
    }
}
