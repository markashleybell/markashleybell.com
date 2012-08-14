using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace markashleybell.com.Models
{
    public class DropboxAccountInfo
    {
        public string referral_link { get; set; }
        public string display_name { get; set; }
        public int uid { get; set; }
        public string country { get; set; }
        public QuotaInfo quota_info { get; set; }
    }

    public class QuotaInfo
    {
        public long shared { get; set; }
        public long quota { get; set; }
        public long normal { get; set; }
    }
}