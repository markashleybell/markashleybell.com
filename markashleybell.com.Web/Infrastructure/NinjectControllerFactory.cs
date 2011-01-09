using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ninject;
using Ninject.Modules;
using System.Configuration;
using markashleybell.com.Domain.Concrete;
using markashleybell.com.Domain.Abstract;

namespace markashleybell.com.Web.Infrastructure
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        // The kernel is the thing that can supply object instances
        private IKernel kernel = new StandardKernel(new NippyNormansServices());

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null) return null;

            return (IController)kernel.Get(controllerType);
        }

        private class NippyNormansServices : NinjectModule
        {
            public override void Load()
            {
                // Configuration here
                Bind<IArticleRepository>()
                    .To<ArticleRepository>()
                    .WithConstructorArgument("databaseFactory", new DbFactory());
            }
        }
    }
}