using MvcIntegrationServerApp.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcIntegrationServerApp.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [Auth.SuperOfficeAuthorize]
        public ActionResult SuperIdLogin()
        {
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult SuperIdLogout()
        {

            // ToDo: Should do federated logout... but it is easier debug if I don't
            SuperOfficeAuthHelper.Logout();
            return RedirectToAction("Index", "Home");
        }

    }
}