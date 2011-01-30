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
        protected IUnitOfWork _unitOfWork;
        protected IArticleRepository _articleRepository;
        protected ICommentRepository _commentRepository;

        public BaseController(IUnitOfWork unitOfWork, IArticleRepository articleRepository, ICommentRepository commentRepository)
        {
            _unitOfWork = unitOfWork;
            _articleRepository = articleRepository;
            _commentRepository = commentRepository;
        }
    }
}
