using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using markashleybell.com.Web.Abstract;
using System.Web.Security;

namespace markashleybell.com.Web.Concrete
{
    public class FormsAuthenticationProvider : IFormsAuthenticationProvider
    {
        public bool Authenticate(string userName, string password)
        {
            return FormsAuthentication.Authenticate(userName, password);
        }

        public void SetAuthCookie(string userName, bool persistent)
        {
            FormsAuthentication.SetAuthCookie(userName, persistent);
        }
    }
}