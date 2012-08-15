using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace markashleybell.com.Models
{
    public class ArticleViewModel
    {
        public string Title { get; set; }
        public DateTime PublishDate { get; set; }
        public string Abstract { get; set; }
        public string Body { get; set; }
        public string Slug { get; set; }
    }
}