using System;
using System.Web.Mvc;
using System.Web.Routing;
using SuperOffice.DevNet.Online.Login;
using SuperOffice.CRM.Services;

namespace SuperOffice.DevNet.Online.SystemUser.Mvc.Controllers
{
    public class ContactEntityController : Controller
    {
        [SuperOfficeAuthorize]
        public ActionResult Index(int id)
        {
            using (var agent = new ContactAgent())
            {
                var contact = agent.GetContactEntity(id);
                return View(contact);
            }
        }
        
        [SuperOfficeAuthorize]
        public ActionResult CreateContactEntity()
        {
            using (var agent = new ContactAgent())
            {
                var timestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                
                var entity = agent.CreateDefaultContactEntity();
                entity.Name = "SuperId-" + timestamp;
                var contact = agent.SaveContactEntity(entity);
                return RedirectToAction("Index", new {id = contact.ContactId});
            }
        }
    }
}