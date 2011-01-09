using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markashleybell.com.Domain.Entities
{
    public class Comment
    {
        public int CommentID { get; set; }

        public string AuthorName { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
        public string Body { get; set; }
        public string BodyMarkdown { get; set; }

        public DateTime Published { get; set; }
        public DateTime Updated { get; set; }

        public virtual Article Article { get; set; }
    }
}
