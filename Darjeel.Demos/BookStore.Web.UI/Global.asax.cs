﻿using System;
using System.Web.Http;

namespace BookStore.Web.UI
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            var container = UnityConfig.GetConfiguredContainer();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            RegistryConfig.Register(container);
            ProcessorConfig.Start(container);
        }
    }
}