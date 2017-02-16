using System;
using System.Web.Mvc;
using System.Web.Routing;
using SuperOffice.DevNet.Online.Login;
using SuperOffice.DevNet.Online.SystemUser.Mvc.Helpers;

namespace SuperOffice.DevNet.Online.SystemUser.Mvc.Controllers
{
    [SuperOfficeAuthorize]
    public class SystemUserController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                var contact = SystemUserHelper.SystemUserTest(SuperOfficeAuthHelper.Context.SystemToken,
                    SuperOfficeAuthHelper.Context.ContextIdentifier);
                return RedirectToAction("Index", "ContactEntity", new {id = contact.ContactId});
            }
            catch (Exception)
            {
                return View();
            }
        }
    }
}