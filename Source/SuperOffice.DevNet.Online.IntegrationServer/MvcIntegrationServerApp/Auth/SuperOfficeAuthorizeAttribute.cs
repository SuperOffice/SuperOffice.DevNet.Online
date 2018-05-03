using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SuperOffice.Factory;

namespace MvcIntegrationServerApp.Auth
{
	/// <summary>
	/// Attribute that can be declared on any action that requires authentication.
	/// </summary>
	public class SuperOfficeAuthorizeAttribute : System.Web.Mvc.ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{

            //if (!SuperOfficeAuthorizeHelper.IsAuthorized(filterContext.HttpContext))
            if (!SuperOfficeAuthHelper.IsAuthorized())
                RedirectToSuperOfficeLogin(filterContext);
		}

		/// <summary>
		/// We must log in to be able to use netserver:
		/// </summary>
		/// <param name="filterContext"></param>
		private void RedirectToSuperOfficeLogin(ActionExecutingContext filterContext)
		{
			SuperOfficeAuthHelper.Context = null;
			var ctx = filterContext.HttpContext.Request.Params["ctx"];
			HttpContext.Current.Session["RedirectUrl"] = filterContext.HttpContext.Request.RawUrl;

			var url = SuperOfficeAuthHelper.GetAuthenticateUrl(ctx);
			filterContext.Result = new RedirectResult(url);
		}
	}
}