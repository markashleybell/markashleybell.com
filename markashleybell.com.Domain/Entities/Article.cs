﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markashleybell.com.Domain.Entities
{
    public class Article
    {
        public int ArticleID { get; set; }

        public string Title { get; set; }
        public string Author { get; set; }
        public string Summary { get; set; }
        public string SummaryHtml { get; set; }
        public string Body { get; set; }
        public string BodyHtml { get; set; }
        public string Slug { get; set; }

        public DateTime Published { get; set; }
        public DateTime Updated { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}
