using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markashleybell.com.Web.Models
{
    public class ArticleViewModel
    {
        public int ArticleID { get; set; }

        public string Title { get; set; }
        public string Author { get; set; }
        public string Summary { get; set; }
        public string SummaryMarkdown { get; set; }
        public string Body { get; set; }
        public string BodyMarkdown { get; set; }
        public string Slug { get; set; }

        public DateTime Published { get; set; }
        public DateTime Updated { get; set; }

        public List<CommentViewModel> Comments = new List<CommentViewModel>();
    }
}
