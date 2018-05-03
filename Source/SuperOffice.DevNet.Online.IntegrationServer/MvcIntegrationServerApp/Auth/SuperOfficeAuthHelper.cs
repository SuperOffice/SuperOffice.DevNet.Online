using System;
using System.Configuration;
using System.Web;
using System.Web.Security;

using SuperOffice.SuperID.Client.Tokens;
using UserType = SuperOffice.License.UserType;
using SuperOffice.Factory;
using SuperOffice;
using SuperOffice.License;
using SuperOffice.Security.Principal;
using SuperOffice.CRM.Services;
using System.IdentityModel.Selectors;
using System.IO;
using System.Web.Hosting;

namespace MvcIntegrationServerApp.Auth
{
    /// <summary>
    /// Untility class supporting SuperOffice Online Authentication
    /// </summary>
    public static class SuperOfficeAuthHelper
    {

        /// <summary>
        /// Is authorized (logged in with NetServer and matching values in cookies and session
        /// </summary>
        /// <returns>True if authorized</returns>
        public static bool IsAuthorized()
        {

            var currentContext = HttpContext.Current;
            var authCookie = currentContext.Request.Cookies[ConfigManager.SoAuthCookie];
            string authSession = currentContext.Session != null ? currentContext.Session[ConfigManager.SoAuthCookie] as string : null;

            // Let's see if we already have logged in.
            // We do this by checking if we find a cookie;
            if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                return false;

            // We found the cookie, let's see if we find the authentication cookie:
            if (authSession != authCookie.Value)
                return false;

            // Ok, we also found the authentication cookie, now let's try to decrypt the value in it:
            var formsTicket = FormsAuthentication.Decrypt(authCookie.Value);
            if (formsTicket == null)
                return false;

            // Now we got a value in the authentication cookie that can be decrypted by the FormsAuthentication.Decrypt method
            // , this means that it was encrypted this computer (using the unique machine code of this computer).
            // Now we must check if we are still autorized with NetServer:
            if (!IsAutorizedWithNetServer())
                return false;

            // Else... everything is ok, and we can Rock ON!
            return true;
        }

        /// <summary>
        /// Is a user authenticated with NetServer
        /// </summary>
        /// <returns></returns>
        internal static bool IsAuthenticatedWithNetServer()
        {
            ClassFactory.Init();

            var principal = SoContext.CurrentPrincipal;
            return IsAuthenticatedWithNetServer(principal);
        }

        /// <summary>
        /// Is the currently authenticated NetServer user authorized to use web and from the correct tenant
        /// </summary>
        /// <returns></returns>
        private static bool IsAutorizedWithNetServer()
        {
            var principal = SoContext.CurrentPrincipal;
            var context = Context;
            return context != null &&
                IsAuthenticatedWithNetServer(principal) &&
                principal.UserType == UserType.InternalAssociate &&
                principal.HasLicense(SoLicenseNames.Web) &&
                String.Equals(SoDatabaseContext.GetCurrent().ContextIdentifier, context.ContextIdentifier, StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool IsAuthenticatedWithNetServer(SoPrincipal principal)
        {
            return principal != null && principal.UserType != UserType.AnonymousAssociate;
        }

        /// <summary>
        /// If the user isn't authiorized yet, the function will redirect the users browser to the online loginpage.
        /// That page will, upon successfull login, redirect to the app's assigned redirect page (as it was reported to SuperOffice OC).
        /// The redirect page should then store the posted claims and redirect back to the first page, it will find the url in Session["RedirectUrl"]
        /// </summary>
        /// <param name="context"></param>
        public static void Authorize()
        {
            string redirUrl;

            if (ShallRedirect(out redirUrl))
            {
                var context = HttpContext.Current;

                context.Response.BufferOutput = true;
                context.Response.Redirect(redirUrl);
                throw new Exception("Should never be called, the system will end execution on the Redirect call. ");
            }
        }

        private static bool ShallRedirect(out string redirectUrl)
        {
            if (!IsAuthorized())
            {
                // No cookie found, go to login.
                redirectUrl = RedirectToSuperOfficeLogin();
                return true;
            }
            // Else... everything is ok, and we can Rock ON!
            redirectUrl = string.Empty;
            return false;
        }

        /// <summary>
        /// We must log in to be able to use netserver:
        /// </summary>
        /// <param name="context"></param>
        private static string RedirectToSuperOfficeLogin()
        {
            var context = HttpContext.Current;

            Context = null;
            var ctx = context.Request.Params["ctx"];
            context.Session["RedirectUrl"] = context.Request.RawUrl;

            var url = GetAuthenticateUrl(ctx);
            return url;
        }



        /// <summary>
        /// Get URL for redirecting to Login Page
        /// </summary>
        /// <param name="customerIdentifier">Optional Context identifier</param>
        /// <returns></returns>
        public static string GetAuthenticateUrl(string customerIdentifier)
        {
            if (String.IsNullOrEmpty(customerIdentifier) || customerIdentifier.ToLower() == "<uctx>")
                customerIdentifier = "";

            return ConfigManager.SoFederationGateway + "?app_id=" + ConfigManager.ApplicationId + "&ctx=" +
                   customerIdentifier;
        }



        /// <summary>
        /// Logout from the MVC application
        /// </summary>
        public static void Logout()
        {
            var context = HttpContext.Current;
            if (context != null && context.Session != null)
            {
                SoContext.CloseCurrentSession();
                context.Session.Abandon();

                context.Response.Cookies.Add(new HttpCookie(ConfigManager.SoAuthCookie, ""));
                context.Response.Cookies.Add(new HttpCookie(ConfigManager.SoSessionCookieName, ""));
            }
        }

        /// <summary>
        /// Get and set internal state from session
        /// </summary>
        public static SuperOfficeContext Context
        {
            get
            {
                var context = HttpContext.Current;
                return context != null && context.Session != null ? context.Session["SoContext"] as SuperOfficeContext : null;
            }
            set
            {
                var context = HttpContext.Current;
                if (context != null && context.Session != null)
                    context.Session["SoContext"] = value;
            }
        }


        public static bool TryLogin(string jwt)
        {
        if (!String.IsNullOrWhiteSpace(jwt))
                return TryLogin(jwt, SuperOffice.SuperID.Contracts.SystemUser.V1.TokenType.Jwt.ToString());
            else return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token">Saml or JWT token</param>
        public static bool TryLogin(string token, string tokenType)
        {
            var tokenHandler = new SuperIdTokenHandler();


            var typedTokenType = (SuperOffice.SuperID.Contracts.SystemUser.V1.TokenType)
                Enum.Parse(typeof(SuperOffice.SuperID.Contracts.SystemUser.V1.TokenType), tokenType);

            var certificatePath = ConfigManager.SuperOfficeFederatedLogin;
            if (!String.IsNullOrWhiteSpace(certificatePath))
            {
                if (!Path.IsPathRooted(certificatePath))
                    certificatePath = Path.Combine(HostingEnvironment.MapPath(@"~"), certificatePath);

                tokenHandler.JwtIssuerSigningCertificate =
                    new System.Security.Cryptography.X509Certificates.X509Certificate2(certificatePath);
            }
            else
            {
                tokenHandler.CertificateValidator = X509CertificateValidator.PeerTrust;
            }

            //tokenHandler.ValidateAudience = false;
            var superIdClaims = tokenHandler.ValidateToken(token, typedTokenType);

            var context = new SuperOfficeContext
            {
                Ticket = superIdClaims.Ticket,
                Email = superIdClaims.Email,
                ContextIdentifier = superIdClaims.ContextIdentifier,
                NetServerUrl = superIdClaims.NetserverUrl,
                SystemToken = superIdClaims.SystemToken,
            };
            Context = context;

            // Use forms authentication - this is optional
            var soFormsTicket = new FormsAuthenticationTicket(superIdClaims.Email, false, 3600);
            var soFormsTicketEncrypted = FormsAuthentication.Encrypt(soFormsTicket);

            var httpContext = HttpContext.Current;
            httpContext.Session[ConfigManager.SoAuthCookie] = soFormsTicketEncrypted;
            httpContext.Response.Cookies.Add(new HttpCookie(ConfigManager.SoAuthCookie, soFormsTicketEncrypted));


            try
            {
                // If request is not authenticated, and a controller with the 
                // SuperOfficeAuthorize attribute is accessed, the called controller
                // will continue to send the user to SuperID. If already authenticated there
                // this user will always return here and be stuck in an endless loop.
                // Therefore, it is important to authenticate with NetServer, and allow the
                // context provider to store the current session. Thus, the SuperOfficeAuthorize
                // attibute will be able to locate the session and proceed unimpeded

                //Authenticate with NetServer using web services if necessary.
                SoSession session = SoSession.Authenticate(new SoCredentials() { Ticket = context.Ticket });

                var principal = SoContext.CurrentPrincipal;
                var contact = new ContactAgent().GetContact(principal.ContactId);

                context.Company = contact.FullName;
                context.Name = principal.FullName;
                context.Username = principal.Associate;
                context.AssociateId = principal.AssociateId;

                return true;
            }
            catch (Exception ex)
            {
                SuperOfficeAuthHelper.Logout();
                return false;
            }
        }
    }
}
