using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace markashleybell.com.Web.Models
{
    public class CommentViewModel
    {
        public int CommentID { get; set; }

        public int ArticleID { get; set; }

        [Required]
        [DisplayName("Your Name")]
        public string AuthorName { get; set; }
        [Required]
        public string Email { get; set; }
        [DisplayName("Your web site (optional)")]
        public string Url { get; set; }
        [Required(ErrorMessage="You must enter a comment")]
        [DisplayName("Comment (you can format comments with Markdown)")]
        public string Body { get; set; }
        public string BodyHtml { get; set; }

        public DateTime Published { get; set; }
        public DateTime Updated { get; set; }
    }
}
