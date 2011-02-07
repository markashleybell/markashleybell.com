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

        [Required(ErrorMessage = "You must enter your name")]
        [DisplayName("Your Name")]
        public string AuthorName { get; set; }
        [Required(ErrorMessage = "You must enter your email address")]
        [RegularExpression(@"^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [DisplayName("Your web site (optional)")]
        public string Url { get; set; }
        [Required(ErrorMessage="You must enter a comment")]
        [DisplayName("Comment")]
        public string Body { get; set; }
        public string BodyHtml { get; set; }
        [Required(ErrorMessage = "Please answer the question")]
        [RegularExpression(@"(16|sixteen|SIXTEEN)", ErrorMessage = "Your answer is incorrect")]
        [DisplayName("Captcha")]
        public string z7sfd602nlwi { get; set; }

        public DateTime Published { get; set; }
        public DateTime Updated { get; set; }
    }
}
