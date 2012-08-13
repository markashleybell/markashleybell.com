using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace markashleybell.com.Models
{
    public class DropboxFile
    {
        public int revision { get; set; }
        public string rev { get; set; }
        public bool thumb_exists { get; set; }
        public int bytes { get; set; }
        public string modified { get; set; }
        public string client_mtime { get; set; }
        public string path { get; set; }
        public bool is_dir { get; set; }
        public string icon { get; set; }
        public string root { get; set; }
        public string mime_type { get; set; }
        public string size { get; set; }
    }
}