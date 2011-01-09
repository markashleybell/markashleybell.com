using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using markashleybell.com.Domain.Abstract;

namespace markashleybell.com.Web.Controllers
{
    public class BaseController : Controller
    {
        protected IArticleRepository _articleRepository;

        public BaseController(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }
    }
}
