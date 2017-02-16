using System.Web.Mvc;
using SuperOffice.DevNet.Online.Login;

namespace SuperOffice.DevNet.Online.SystemUser.Mvc.Controllers
{
    public class HomeController : Controller
	{
        [SuperOfficeAuthorize]
        public ActionResult Index()
		{
            return View();
		}

        [AllowAnonymous]
        public ActionResult Welcome()
        {
            return View();
        }
	}
}