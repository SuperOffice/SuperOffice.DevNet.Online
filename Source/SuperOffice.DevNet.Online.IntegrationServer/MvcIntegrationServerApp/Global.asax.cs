using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using System.Data.Entity;
using MvcIntegrationServerApp.Models;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Runtime;


namespace MvcIntegrationServerApp
{
    public class Global : HttpApplication
    {
        public static IConfiguration Configuration { get; private set; }
        public static AppSettings Settings { get; private set; }

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // Read from appsettings
            var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();

            // Bind the configuration to your model
            Settings = Configuration.GetSection("Appsettings").Get<AppSettings>();

            //Database.SetInitializer<AppDB>(null);
            //AppDB.Initialize();
            //AppDB.UpgradeDatabase();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }
    }
}