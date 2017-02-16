using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SuperOffice.Factory;
using SuperOffice.DevNet.Online.Login.Repository;

namespace SuperOffice.DevNet.Online.Login
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
            var crmAppsKey = filterContext.HttpContext.Request.Params["CrmAppsKey"];
            HttpContext.Current.Session["RedirectUrl"] = filterContext.HttpContext.Request.RawUrl;

            //ToDo: Lookup up request and get customer id
            if (!string.IsNullOrEmpty(crmAppsKey))
            {
                var customer = CustomerDataSource.Instance.Customers.Find(c=>c.CustomerKey == crmAppsKey);
                if (customer != null)
                {
                    if (customer.IsOnlineCustomer.HasValue && !customer.IsOnlineCustomer.Value)
                    {
                        //Is OnSite customer
                        var ticket = filterContext.HttpContext.Request.Params["usec"];
                        var userId = filterContext.HttpContext.Request.Params["userId"];
                        var redirectUrl = "~/Security/LoginOnSite?ticket=" + ticket + "&customerId=" + crmAppsKey + "&userId=" + userId;
                        filterContext.Result = new RedirectResult(redirectUrl);
                        return;
                    }
                }
            }

            //ToDo: Ask CustomerContext if this is on premise
            //ToDo: Redirect to local login controller for onSite


            //Default is SuperOffice Onlne
            var url = SuperOfficeAuthHelper.GetAuthenticateUrl(ctx);
			filterContext.Result = new RedirectResult(url);
		}
	}
}