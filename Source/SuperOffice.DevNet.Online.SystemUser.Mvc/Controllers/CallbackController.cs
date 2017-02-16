using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SuperOffice.DevNet.Online.SystemUser.PartnerDBLibrary.Models;
using SuperOffice.Configuration;
using SuperOffice.DevNet.Online.Login;
using SuperOffice.Security.Principal;
using SuperOffice.SuperID.Client.Tokens;
using SuperOffice.SuperID.Contracts.SystemUser.V1;

namespace SuperOffice.DevNet.Online.SystemUser.Mvc.Controllers
{
	public class CallbackController : Controller
	{
	    /// <summary>
	    /// Action called on successfull authentication by NetServer.
	    /// </summary>
	    /// <param name="callbackModel"></param>
	    /// <returns></returns>
	    [HttpPost]
	    [ValidateInput(false)]
	    public ActionResult Index(CallbackModel callbackModel)
	    {
	        if (callbackModel == null)
	            return RedirectToAction("Index", "Home");



            /*
			 * Here it is up to the partner intercept the callback from SuperID 
			 * and route the user to the correct partner application instance.
			 * 
			 * This is also the opportunity for the Partner to create a system user
			 * in the customers superoffice database for future use and storage.
			 * 
			 * This is where any additional setup or configuration options are input into 
			 * the partners application for future use.
			 */

            string error = string.Empty;

	        if (SuperOfficeAuthHelper.TryLogin(callbackModel, out error))
	        {
	            var context = SuperOfficeAuthHelper.Context;
	            //Store the System User Information in the Database
                CustomerDataSource dataSource = new CustomerDataSource();
                 var customer = dataSource.Customers.FirstOrDefault(c => c.ContextIdentifier == context.ContextIdentifier);

                //var databaseContext = new PartnerDatabaseContext();
                //var customer = databaseContext.Customers.FirstOrDefault(c => c.ContextIdentifier == context.ContextIdentifier);
	            if (customer == null)
	            {
                    dataSource.Customers.Add(new CustomerInfo
	                {
                        AssociateID = context.AssociateId,
                        ContextIdentifier = context.ContextIdentifier,
	                    IsActive = true,
	                    LastSync = new DateTime(2000, 1, 1),
                        SystemUserToken = context.SystemToken
	                });
                    dataSource.Save();
	            }

	            // Redirect to original request
	            var redirectUr = Session["RedirectUrl"] as string;

	            if (!String.IsNullOrEmpty(redirectUr))
	                return Redirect(redirectUr);
	            else
	                return RedirectToAction("Index", "Home");

	        }
	        else
	        {
	            return RedirectToAction("Welcome", "Home", new { Error = error });

	        }


	    }
	}
}