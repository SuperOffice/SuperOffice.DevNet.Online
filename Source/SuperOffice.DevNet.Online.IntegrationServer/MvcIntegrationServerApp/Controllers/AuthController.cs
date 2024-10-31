using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Discovery;
using Microsoft.IdentityModel.Tokens;
using MvcIntegrationServerApp.Controllers;

namespace MvcIntegrationServerApp.Auth
{
    public class AuthController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> Login()
        {
            // Get Discovery Document
            await OAuthHelper.InitializeOpenIdConnectEndpointsAsync();
            string authorizeUrl = OAuthHelper.CreateAuthorizeUrl(Session, "");
            return Redirect(authorizeUrl);
        }

        public async Task<ActionResult> Callback(string code, string state)
        {
            // Retrieve the stored state from session or secure cookie
            var storedState = Session["state"]?.ToString();
            if (string.IsNullOrEmpty(storedState) || storedState != state)
            {
                // Handle invalid state (e.g., log an error and return an error response)
                return RedirectToAction("Error", new { message = "Invalid state parameter." });
            }

            // Clear the stored state to prevent reuse
            Session.Remove("state");

            if (string.IsNullOrEmpty(code))
            {
                return RedirectToAction("Error"); // Handle error appropriately
            }

            await OAuthHelper.GetAuthorizationCode(Session, code);

            // Redirect to a secure page or home
            return RedirectToAction("Index", "Home");
        }
    }
}