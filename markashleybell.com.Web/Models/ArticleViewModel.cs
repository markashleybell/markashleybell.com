using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace markashleybell.com.Web.Models
{
    public class ArticleViewModel
    {
        public int ArticleID { get; set; }

        [Required]
        public string Title { get; set; }
        public string Author { get; set; }
        public string Summary { get; set; }
        public string SummaryHtml { get; set; }
        [Required]
        public string Body { get; set; }
        public string BodyHtml { get; set; }
        public string Slug { get; set; }

        public DateTime Published { get; set; }
        public DateTime Updated { get; set; }

        public List<CommentViewModel> Comments = new List<CommentViewModel>();
    }
}
