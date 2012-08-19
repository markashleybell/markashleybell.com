using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace markashleybell.com.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult ServerError()
        {
            return View();
        }

        public ActionResult PageNotFound()
        {
            return View();
        }
    }
}
