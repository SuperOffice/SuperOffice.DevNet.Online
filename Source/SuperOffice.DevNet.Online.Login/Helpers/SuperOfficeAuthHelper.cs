using System;
using System.Configuration;
using System.IdentityModel.Selectors;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Security;
using SuperOffice;
using SuperOffice.License;
using SuperOffice.Security.Principal;
using SuperOffice.Services75;
using SuperOffice.SuperID.Client.Tokens;
using SuperOffice.SuperID.Contracts.SystemUser.V1;
using UserType = SuperOffice.License.UserType;
using SuperOffice.Factory;
using SuperOffice.DevNet.Online.Login.Repository;
using Newtonsoft.Json.Linq;

namespace SuperOffice.DevNet.Online.Login
{

    /// <summary>
    /// Untility class supporting SuperOffice Online Authentication
    /// </summary>
    public static class SuperOfficeAuthHelper
    {
        private static SuperOfficeContext _contextContainer;
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

            //Extra check - 1.nov 2015, Frode.
            //Is the surrounding CRM Online client the same as the one we're already authorized with?
            //If the user has logged in to one database, we've authenticated, the user logs out of SuperOffice, and into another database,
            //then we need to re-authenticate in our webpanel as well.
            //We're attempting to solve this by adding &ctx=<uctx> in the URL. If it exists in the URL, then store it in the session. Then compare the
            //value from the Session with the one from our authenticated Session. If they match, then all is good. If not, then reauthenticate.
            var ctxFromUrl = currentContext.Request["ctx"];
            var customerKeyFromUrl = currentContext.Request["CrmAppsKey"];

            var differentCustomerKey = !String.IsNullOrWhiteSpace(customerKeyFromUrl) && (Context.CustomerKey != customerKeyFromUrl);
            var differentContextIdentifier = !String.IsNullOrWhiteSpace(ctxFromUrl) && (Context.ContextIdentifier != ctxFromUrl);
            //TODO - is this the same AssociateId
            if (differentContextIdentifier && differentCustomerKey)
            {
                Logout();
                return false;
            }

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
        /// Is the currently authenticated NetServer user authorized to use web and from the correct tenant
        /// </summary>
        /// <returns></returns>
        private static bool IsAutorizedWithNetServer()
        {
            var principal = SoContext.CurrentPrincipal;

            if (Context == null)
                return false;

            if (!IsAuthenticatedWithNetServer(principal))
                return false;

            if (principal.UserType != UserType.InternalAssociate)
                return false;

            if (!principal.HasLicense(SoLicenseNames.Web))
                return false;

            if (!String.Equals(SoDatabaseContext.GetCurrent().ContextIdentifier, Context.ContextIdentifier, StringComparison.InvariantCultureIgnoreCase))
                return false;

            //All checks were successful.
            return true;
        }

        private static bool IsAuthenticatedWithNetServer(SoPrincipal principal)
        {
            return principal != null && principal.UserType != UserType.AnonymousAssociate;
        }

        ///// <summary>
        ///// If the user isn't authiorized yet, the function will redirect the users browser to the online loginpage.
        ///// That page will, upon successfull login, redirect to the app's assigned redirect page (as it was reported to SuperOffice OC).
        ///// The redirect page should then store the posted claims and redirect back to the first page, it will find the url in Session["RedirectUrl"]
        ///// </summary>
        ///// <param name="context"></param>
        //public static void Authorize()
        //{
        //    string redirUrl;

        //    if (ShallRedirect(out redirUrl))
        //    {
        //        var context = HttpContext.Current;

        //        context.Response.BufferOutput = true;
        //        context.Response.Redirect(redirUrl);
        //        throw new Exception("Should never be called, the system will end execution on the Redirect call. ");
        //    }
        //}

        //private static bool ShallRedirect( out string redirectUrl)
        //{
        //    if (!IsAuthorized())
        //    {
        //        // No cookie found, go to login.
        //        redirectUrl = RedirectToSuperOfficeLogin();
        //        return true;
        //    }
        //    // Else... everything is ok, and we can Rock ON!
        //    redirectUrl = string.Empty;
        //    return false;
        //}

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
        public static string GetAuthenticateUrl(string contextIdentifier)
        {
            if (String.IsNullOrEmpty(contextIdentifier) || contextIdentifier.ToLower() == "<uctx>")
                contextIdentifier = "";

            if(!ConfigManager.UseOidc)
            {
                var urlResult = ConfigManager.SoFederationGateway + "?app_id=" + ConfigManager.AppId;

                var redirectUri = ConfigManager.CallbackURL;
                if (!string.IsNullOrEmpty(redirectUri))
                {
                    urlResult += "&redirect_uri=" + redirectUri;
                }

                return urlResult += "&ctx=" + contextIdentifier;
            }
            
            string state = Guid.NewGuid().ToString();
            HttpContext.Current.Session["state"] = state;

            //https://sod.superoffice.com/login/
            //{ contextIdentifier}/oauth/authorize
            //?response_type=id_token token
            //&client_id = YOUR - APP - ID & redirect_uri = YOUR - REDIRECT - URL & scope = openid
            //& state = 12345 & nonce = 7362CAEA - 9CA5 - 4B43 - 9BA3 - 34D7C303EBA7
            if (string.IsNullOrEmpty(contextIdentifier))
            {
                return ConfigManager.SoFederationGateway + "common/oauth/authorize?response_type=code&client_id=" + ConfigManager.AppId + "&redirect_uri=" + ConfigManager.CallbackURL + "&response_mode=form_data&scope=openid&state=" + state;
            }
            else
            {
                return ConfigManager.SoFederationGateway + contextIdentifier + "/common/oauth/authorize?response_type=code&client_id=" + ConfigManager.AppId + "&redirect_uri=" + ConfigManager.CallbackURL + "&response_mode=form_data&scope=openid&state=" + state;
            }
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

        public static bool TryLogin(OidcModel model, out string errorReason)
        {
            return TryLogin(new CallbackModel() { Jwt = model.IdToken}, model, out errorReason);
        }

        public static bool TryLogin(CallbackModel model, out string errorReason)
        {
            return TryLogin(model, null, out errorReason);
        }

        public static bool TryLogin(CallbackModel model, OidcModel oidcModel, out string errorReason)
        {
            
            if (!String.IsNullOrEmpty(model.Jwt))
                return TryLogin(model.Jwt, SuperOffice.SuperID.Contracts.SystemUser.V1.TokenType.Jwt.ToString(), oidcModel, out errorReason);
            else if (!String.IsNullOrEmpty(model.Saml))
                return TryLogin(model.Saml, SuperOffice.SuperID.Contracts.SystemUser.V1.TokenType.Saml.ToString(), oidcModel, out errorReason);
            else
            {
                errorReason = "SAML and JWT empty.";
                return false;
            }
        }


        /// <summary>
        /// Used by Online
        /// </summary>
        /// <param name="token">Saml or JWT token</param>
        public static bool TryLogin(string token, string tokenType, OidcModel oidcModel, out string errorReason)
        {
            errorReason = String.Empty;
            var tokenHandler = new SuperIdTokenHandler();
            tokenHandler.ValidIssuer = oidcModel == null ? "SuperOffice AS" : "https://sod.superoffice.com"; // required for OIDC vs. Old Federated Auth...

            var useAppData = Convert.ToBoolean(ConfigurationManager.AppSettings["CertificatesInAppDataFolder"]);

            var typedTokenType = (SuperOffice.SuperID.Contracts.SystemUser.V1.TokenType)
                Enum.Parse(typeof(SuperOffice.SuperID.Contracts.SystemUser.V1.TokenType), tokenType);

            if (useAppData && typedTokenType == SuperOffice.SuperID.Contracts.SystemUser.V1.TokenType.Saml)
            {
                tokenHandler.IssuerTokenResolver = 
                    new CertificateFileCertificateStoreTokenResolver(
                        HttpContext.Current.Server.MapPath("~/App_Data"));
                tokenHandler.CertificateValidator = X509CertificateValidator.None;
            }
            else if(useAppData && typedTokenType == SuperOffice.SuperID.Contracts.SystemUser.V1.TokenType.Jwt)
            {
                tokenHandler.JwtIssuerSigningCertificate = 
                    new System.Security.Cryptography.X509Certificates.X509Certificate2(
                        HttpContext.Current.Server.MapPath("~/App_Data/") + "SuperOfficeFederatedLogin.crt");
            }
            else
            {
                tokenHandler.CertificateValidator = X509CertificateValidator.PeerTrust;
            }

            tokenHandler.ValidateAudience = false;
            var superIdClaims = tokenHandler.ValidateToken(token, typedTokenType);

            var context = new SuperOfficeContext
            {
                Ticket = superIdClaims.Ticket,
                Email = superIdClaims.Email,
                ContextIdentifier = superIdClaims.ContextIdentifier,
                NetServerUrl = superIdClaims.NetserverUrl,
                SystemToken = superIdClaims.SystemToken,
                CustomerKey = String.Empty,
                IsOnSiteCustomer = false,
                AccessToken = oidcModel?.AccessToken,
                IdToken = oidcModel?.IdToken,
                RefreshToken = oidcModel?.RefreshToken
            };
            return TryLogin(context, out errorReason);

        }

        /// <summary>
        /// Used by OnSite customers only
        /// </summary>
        /// <param name="customerKey"></param>
        /// <param name="ticket"></param>
        /// <param name="userId"></param>
        /// <param name="errorReason"></param>
        /// <returns></returns>
        public static bool TryLogin(string customerKey, string ticket, string userId, bool local, out string errorReason)
        {
            if (String.IsNullOrEmpty(customerKey))
            {
                errorReason = "CustomerKey is missing.";
                return false;
            }

            var customer = CustomerDataSource.Instance.Customers.Find( c => c.CustomerKey == customerKey);

            if (customer == null)
            {
                errorReason = "Could not find customer: " + customerKey;
                return false;
            }

            var netServerUrl = customer.NetServerUrl;
            if (String.IsNullOrEmpty(netServerUrl))
            {
                errorReason = "No NetServerUrl";
                return false;
            }

            var context = new SuperOfficeContext
            {
                Ticket = ticket,
                Email = userId,
                ContextIdentifier = customer.ContextIdentifier,
                NetServerUrl = netServerUrl + "Services75/",
                CustomerKey = customer.CustomerKey,
                IsOnSiteCustomer = true,
                WebClientUrl = customer.WebClientUrl
            };

            return TryLogin(context, out errorReason);
        }

        /// <summary>
        /// Does the actual authentication
        /// </summary>
        /// <param name="context"></param>
        /// <param name="errorReason"></param>
        /// <returns></returns>
        public static bool TryLogin(SuperOfficeContext context, out string errorReason)
        {
            Context = context;

            // Use forms authentication - this is optional
            var soFormsTicket = new FormsAuthenticationTicket(context.Email, false, 3600);
            var soFormsTicketEncrypted = FormsAuthentication.Encrypt(soFormsTicket);

            var httpContext = HttpContext.Current;
            httpContext.Session[ConfigManager.SoAuthCookie] = soFormsTicketEncrypted;
            httpContext.Response.Cookies.Add(new HttpCookie(ConfigManager.SoAuthCookie, soFormsTicketEncrypted));

            //If we are allready authorized, then logout first.
            if (SuperOffice.SoContext.IsAuthenticated)
            {
                SuperOffice.SoContext.CloseCurrentSession();
                SuperOfficeAuthHelper.Logout();
            }

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

                /*
                //    From Jens on DevNet: 
                //    The SuperOffice.Configuration.ConfigFile.WebServices.RemoteBaseUrl is the value actually being used by the proxy to communicate with the server.  
                //    This value is read from SuperOffice.Configuration.ConfigFile.Services.RemoteBaseUrl if it is not defined.
                //    The values of SuperOffice.Configuration.ConfigFile.Services are stored in a value that is static throughout the NetServer process, 
                //    and shared between tenants in a multi-tenant configuration.  
                //    The values SuperOffice.Configuration.ConfigFile.WebServices are tenant specific configuration values.
                //    */
                //}

                SoSession session = null; 

                if (string.IsNullOrEmpty(context.AccessToken))
                {
                    session = SoSession.Authenticate(new SoCredentials() { Ticket = context.Ticket });

                }
                else
                {
                    session = SoSession.Authenticate(new SoAccessTokenSecurityToken(context.AccessToken));
                }

                var principal = SoContext.CurrentPrincipal;
                OverrideContextIdentifier(principal, context.ContextIdentifier);
                var contact = new ContactAgent().GetContact(principal.ContactId);

                context.Company = contact.FullName;
                context.Name = principal.FullName;
                context.Username = principal.Associate;
                context.AssociateId = principal.AssociateId;

                errorReason = String.Empty;

                return true;
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }

                errorReason = ex.Message;
                SuperOfficeAuthHelper.Logout();
                return false;
            }
        }

        //ContextIdentifier in multitenant apps is not the same as Default context in single tenant OnPremise customer.
        public static void OverrideContextIdentifier(SoPrincipal principal, string contextIdentifier)
        {
            var carrier = SuperOffice.Security.Principal.Private.PrincipalHelper.GetPrincipalCarrier(principal);
            carrier.DatabaseContextIdentifier = contextIdentifier;
        }

        public static OidcModel GetOAuthModel(string code)
        {
            HttpClient client = new HttpClient();
            var parameters = new System.Collections.Generic.Dictionary<string, string> {
                { "client_id", ConfigManager.AppId },
                { "client_secret", ConfigManager.AppToken },
                { "code", code },
                { "redirect_uri", ConfigManager.CallbackURL },
                { "grant_type", "authorization_code" }
            };
            var encodedContent = new FormUrlEncodedContent(parameters);

            HttpResponseMessage response = client.PostAsync(ConfigManager.SoFederationGateway + "common/oauth/tokens", encodedContent).GetAwaiter().GetResult();
            var responseStream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
            System.IO.StreamReader readStream = new System.IO.StreamReader(responseStream, System.Text.Encoding.UTF8);
            var content = readStream.ReadToEnd();

            var json = JObject.Parse(content);
            string accessToken = json["access_token"].Value<string>();
            string idToken = json["id_token"].Value<string>();
            string refreshToken = json["refresh_token"].Value<string>();

            return new OidcModel() { IdToken = idToken, AccessToken = accessToken, RefreshToken = refreshToken };

        }
    }
}