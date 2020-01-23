using System;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SuperOffice.DevNet.Online.SystemUser.Mvc
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters( GlobalFilters.Filters );
			RouteConfig.RegisterRoutes( RouteTable.Routes );
			BundleConfig.RegisterBundles( BundleTable.Bundles );

			AntiForgeryConfig.SuppressXFrameOptionsHeader = true;
		}

		protected void Session_Start(object sender, EventArgs e)
		{
		}

		protected void Session_End(object sender, EventArgs e)
		{
		}

	}
}
