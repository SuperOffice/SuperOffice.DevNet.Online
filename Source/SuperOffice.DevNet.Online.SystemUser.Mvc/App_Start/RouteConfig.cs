using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace SuperOffice.DevNet.Online.SystemUser.Mvc
{
	public class RouteConfig
	{
		public static void RegisterRoutes( RouteCollection routes )
		{
			routes.IgnoreRoute( "{resource}.axd/{*pathInfo}" );

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "SuperOffice.DevNet.Online.SystemUser.Mvc.Controllers" }
			);
		}
	}
}
