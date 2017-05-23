using SportsStore.Domain.Entities;
using SportsStore.WebUI.Infrastructure.Binders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SportsStore.WebUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //告诉Mvc框架，使用CartModelBinder来创建Cart实例
            ModelBinders.Binders.Add(typeof(Cart), new CartModelBinder());
        }
    }
}
