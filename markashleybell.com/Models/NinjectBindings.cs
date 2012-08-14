using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject.Modules;
using System.Web.SessionState;

namespace markashleybell.com.Models
{
    public class NinjectBindings : NinjectModule
    {
        public override void Load()
        {
            this.Bind<HttpSessionStateBase>().ToMethod(x => new HttpSessionStateWrapper(HttpContext.Current.Session));
        }
    }
}