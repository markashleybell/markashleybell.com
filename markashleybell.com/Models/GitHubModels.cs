using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace markashleybell.com.Models
{
    public class Actor
    {
        public string login { get; set; }
        public string url { get; set; }
    }

    public class Author
    {
        public string name { get; set; }
        public string email { get; set; }
    }

    public class Commit
    {
        public string message { get; set; }
        public Author author { get; set; }
        public string url { get; set; }
        public string sha { get; set; }
    }

    public class PullRequest
    {
        public object diff_url { get; set; }
        public object patch_url { get; set; }
        public object html_url { get; set; }
    }

    public class Issue
    {
        public int number { get; set; }
        public User user { get; set; }
        public string html_url { get; set; }
        public string state { get; set; }
        public PullRequest pull_request { get; set; }
        public object assignee { get; set; }
        public string body { get; set; }
        public int id { get; set; }
        public string title { get; set; }
    }

    public class User
    {
        public string login { get; set; }
        public string url { get; set; }
    }

    public class Comment
    {
        public string created_at { get; set; }
        public User user { get; set; }
        public string updated_at { get; set; }
        public string body { get; set; }
        public int id { get; set; }
        public string url { get; set; }
    }

    public class Payload
    {
        public string @ref { get; set; }
        public List<Commit> commits { get; set; }
        public Issue issue { get; set; }
        public string action { get; set; }
        public Comment comment { get; set; }
        public string description { get; set; }
    }

    public class Repo
    {
        public string url { get; set; }
        public string name { get; set; }
        public int id { get; set; }
    }

    public class Activity
    {
        public string created_at { get; set; }
        public string type { get; set; }
        public Payload payload { get; set; }
        public bool @public { get; set; }
        public Repo repo { get; set; }
        public string id { get; set; }
    }

    public class ActivitySummary 
    {
        public string date { get; set; }
        public string repo { get; set; }
        public string repo_url { get; set; }
        public string status { get; set; }
    }
}