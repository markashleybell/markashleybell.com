using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace markashleybell.com.Models
{
    public class DropboxFolder
    {
        public string hash { get; set; }
        public bool thumb_exists { get; set; }
        public int bytes { get; set; }
        public string path { get; set; }
        public bool is_dir { get; set; }
        public string size { get; set; }
        public string root { get; set; }
        public List<DropboxFile> contents { get; set; }
        public string icon { get; set; }
    }
}