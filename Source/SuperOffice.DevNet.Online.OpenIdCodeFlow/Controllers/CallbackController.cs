using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcTest.Controllers
{
    public class CallbackController : Controller
    {
        // GET: /Callback

        // GET/POST: Home/LoginRedirect?saml=xxx&jwt=yyy
        //
        // SuperId calls this with OAuth tokens
        // See SoAppUrl in web.config: https://localhost/zuora/callback
        //
        // See https://community.superoffice.com/en/crm-online/Partners-and-App-Store/how-to-develop-on-the-superoffice-online-platform/building-your-first-application/Security-and-Authentication/online-superoffice-online-open-id-connect/
        public ActionResult Index(string state, string code)
        {
            // Callback 1 in authorization flow
            if (state != null && code != null)
            {
                try
                {
                    var storedState = Session["state"] as string;
                    if (storedState != state)
                        throw new Exception("OAuth State mismatch: " + storedState + " vs " + state);

                    OAuthHelper.GetAuthorizationCode(Server, Session, code);

                    return RedirectToAction("Index", "App");
                }
                catch( Exception ex)
                {
                    object model = ex.Message;
                    return View(model);
                }
            }

            // Something is wrong, start over
            object error = "Unknown callback request. Missing state and/or code.";
            return View( error );
        }
    }
}