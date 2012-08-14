using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject.Modules;
using Ninject.Web.Common;
using System.Web.SessionState;

namespace markashleybell.com.Models
{
    public class NinjectBindings : NinjectModule
    {
        public override void Load()
        {
            //Bind<HttpContextBase>().ToMethod(context => new HttpContextWrapper(HttpContext.Current));
        }
    }
}