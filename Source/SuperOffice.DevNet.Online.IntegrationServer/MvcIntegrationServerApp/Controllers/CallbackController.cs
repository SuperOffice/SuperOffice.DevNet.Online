using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SuperOffice.SuperID.Client.Tokens;
using SuperOffice.SuperID.Contracts.SystemUser.V1;
using MvcIntegrationServerApp.Auth;
using MvcIntegrationServerApp.Models;

namespace MvcIntegrationServerApp.Controllers
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
        public ActionResult Index(string jwt)
        {
            if (String.IsNullOrWhiteSpace(jwt))
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


            if (SuperOfficeAuthHelper.TryLogin(jwt))
            {
                var context = SuperOfficeAuthHelper.Context;

                AppDB.Initialize();
                AppDB.UpgradeDatabase();


                using (var db = new AppDB())
                {
                    bool newCustomer = false;
                    var utcNow = DateTime.UtcNow;
                    User user = null;

                    var customer = db.Customers.FirstOrDefault(c => c.ContextIdentifier == context.ContextIdentifier);
                    if (customer == null)
                    {
                        customer = new Customer
                        {
                            ContextIdentifier = context.ContextIdentifier,
                            Registered = utcNow,
                            LastUsed = utcNow,
                        };
                        db.Customers.Add(customer);
                        newCustomer = true;
                        db.SaveChanges(); // Save one first....
                    }

                    if (!newCustomer)
                    {
                        user = customer.Users.FirstOrDefault(u => u.AssociateId == context.AssociateId);
                    }

                    if (user == null)
                    {
                        user = new User
                        {
                            Customer = customer,
                            AssociateId = context.AssociateId,
                        };
                        db.Users.Add(user);
                    }
                    user.UserPrincipalName = context.Username;
                    user.Email = context.Email;

                    if (newCustomer)
                    {
                        customer.RegisteredBy = user;
                    }

                    customer.LastUsedBy = user;
                    customer.LastUsed = utcNow;
                    customer.Name = context.Company;
                    customer.NetServerUrl = context.NetServerUrl;
                    customer.SystemUserToken = context.SystemToken;


                    db.SaveChanges();
                    SystemUserManager.ClearCachedItem(context.ContextIdentifier);


                }
                //Store the System User Information in the Database
                //var databaseContext = new PartnerDatabaseContext();
                //var customer = databaseContext.Customers.FirstOrDefault(c => c.ContextIdentifier == context.ContextIdentifier);
                //if (customer == null)
                //{
                //    databaseContext.Customers.Add(new CustomerInfo
                //    {
                //        AssociateID = context.AssociateId,
                //        ContextIdentifier = context.ContextIdentifier,
                //        IsActive = true,
                //        LastSync = new DateTime(2000, 1, 1),
                //        SystemUserToken = context.SystemToken
                //    });
                //    databaseContext.SaveChanges();
                //}

                // Redirect to original request
                var redirectUr = Session["RedirectUrl"] as string;

                if (!String.IsNullOrEmpty(redirectUr))
                    return Redirect(redirectUr);
                else
                    return RedirectToAction("Index", "Home");

            }
            else
            {
                return RedirectToAction("Index", "Home");

            }


        }
    }
}

