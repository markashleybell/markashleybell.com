using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace markashleybell.com.Models
{
    public interface IFormsAuthenticationProvider
    {
        bool Authenticate(string userName, string password);
        void SetAuthCookie(string userName, bool persistent);
        void SignOut();
    }
}