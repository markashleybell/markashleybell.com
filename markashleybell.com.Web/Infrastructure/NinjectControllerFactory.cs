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
using markashleybell.com.Web.Abstract;
using markashleybell.com.Web.Concrete;

namespace markashleybell.com.Web.Infrastructure
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        // The kernel is the thing that can supply object instances
        private IKernel kernel = new StandardKernel(new NinjectConfig());

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null) return null;

            return (IController)kernel.Get(controllerType);
        }

        private class NinjectConfig : NinjectModule
        {
            public override void Load()
            {
                Bind<IUnitOfWork>()
                    .To<UnitOfWork>()
                    .InRequestScope()
                    .WithConstructorArgument("databaseFactory", new DbFactory());

                Bind<IArticleRepository>()
                    .To<ArticleRepository>();

                Bind<ICommentRepository>()
                    .To<CommentRepository>();

                Bind<IFormsAuthenticationProvider>()
                    .To<FormsAuthenticationProvider>();
            }
        }
    }
}