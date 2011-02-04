using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace markashleybell.com.Domain.Entities
{
    public class Article
    {
        public int ArticleID { get; set; }

        public string Title { get; set; }
        public string Author { get; set; }
        [Column(TypeName = "ntext")]
        public string Summary { get; set; }
        [Column(TypeName = "ntext")]
        public string SummaryHtml { get; set; }
        [Column(TypeName = "ntext")]
        public string Body { get; set; }
        [Column(TypeName = "ntext")]
        public string BodyHtml { get; set; }
        public string Slug { get; set; }

        public DateTime Published { get; set; }
        public DateTime Updated { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}
