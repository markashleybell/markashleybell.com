using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace markashleybell.com.Web.Models
{
    public class ArticleDetailPageViewModel
    {
        public int ArticleID { get; set; }

        public string Title { get; set; }
        public string Author { get; set; }
        public string BodyHtml { get; set; }
        public string Slug { get; set; }
        public DateTime Published { get; set; }
        public DateTime Updated { get; set; }

        public List<CommentViewModel> Comments = new List<CommentViewModel>();

        public CommentViewModel Comment { get; set; }
    }
}
