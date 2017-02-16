using System;
using System.Configuration;
using System.IdentityModel.Claims;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web.Hosting;
using SuperOffice;
using SuperOffice.Configuration;
using SuperOffice.CRM.Services;
using SuperOffice.Security.Principal;
using SuperOffice.SuperID.Client.Tokens;
using SuperOffice.SuperID.Contracts.SystemUser.V1;


namespace SuperOffice.DevNet.Online.SystemUser.Mvc.Helpers
{
    public class SystemUserHelper
    {
        /// <summary>
        /// Obtain system user credentials and do work as the system user in another thread 
        /// </summary>
        /// <remarks>
        ///     A diferent thread must be used for working with system users to comply with the session state
        ///     concept in PartnerHttpContext.
        /// </remarks>
        /// <param name="userToken"></param>
        /// <param name="contextIdentifier"></param>
        /// <returns></returns>
        public static ContactEntity SystemUserTest(string userToken, string contextIdentifier)
        {
            ContactEntity retVal = null;
            var token = GetSystemUserToken(userToken, contextIdentifier);

            var thread = new Thread(() => retVal = DoWorkAsSystemUser(token)) {IsBackground = true};
            thread.Start();

            bool completed = thread.Join(300*1000);
            if (!completed)
            {
                thread.Abort();
            }

            return retVal;
        }

        /// <summary>
        /// Get and validate claims for the system user from the SuperID web service
        /// </summary>
        /// <param name="userToken">System user token, not yet signed</param>
        /// <param name="contextIdentifier">Context identifier of the customer</param>
        /// <returns>Token with claims</returns>
        private static SuperIdToken GetSystemUserToken(string userToken, string contextIdentifier)
        {
            var systemToken = new SystemToken(userToken);

            // Get certificate
            var certificatePath = CertificatePath;

            // sign the system user token
            var signedSystemToken = systemToken.Sign(privateKey: File.ReadAllText(certificatePath));

            // Call the web service to exchange signed system user token with claims for the system user
            var federationGateway = ConfigurationManager.AppSettings["SoFederationGateway"];
            var returnedToken = systemToken.AuthenticateWithSignedSystemToken(federationGateway, signedSystemToken,
                ConfigFile.Services.ApplicationToken, contextIdentifier, TokenType.Saml);

            // Validate and return SuperId token for the system user
            var tokenHandler = new SuperIdTokenHandler();

            tokenHandler.IssuerTokenResolver = 
                new SuperOffice.SuperID.Client.Tokens.CertificateFileCertificateStoreTokenResolver(
                System.Web.HttpContext.Current.Server.MapPath("~/App_Data")
                );

            tokenHandler.CertificateValidator = System.IdentityModel.Selectors.X509CertificateValidator.None;

            return tokenHandler.ValidateToken(returnedToken, TokenType.Saml);
        }



        /// <summary>
        /// Use system user token to perform work
        /// </summary>
        /// <param name="token">Token returned from SuperId and validated</param>
        /// <returns>Contact entity created as the system user</returns>
        private static ContactEntity DoWorkAsSystemUser(SuperIdToken token)
        {
            // Enter database context for the customer (enter the right multi-tenant context)
            using (var context = SoDatabaseContext.EnterDatabaseContext(token.ContextIdentifier))
            {
                // set appropriate url for the customer tenant
                ConfigFile.Services.RemoteBaseURL = token.NetserverUrl;

                // Log in as the system user
                using (var session = SoSession.Authenticate(new SoCredentials(token.Ticket)))
                {
                    // Do work as the system user
                    var principal = SoContext.CurrentPrincipal;
                    System.Diagnostics.Trace.WriteLine(principal.Associate);
                    using (var agent = new ContactAgent())
                    {
                        var timestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                        var entity = agent.CreateDefaultContactEntity();
                        entity.Name = "SuperId-" + timestamp;
                        return agent.SaveContactEntity(entity);
                        
                    }
                }
            }

        }


        /// <summary>
        /// Helper for obtaining certificate path
        /// </summary>
        private static string CertificatePath
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                var assemblyDir = Path.GetDirectoryName(path);
                
                var configPath = ConfigurationManager.AppSettings["SystemTokenCertificatePath"];
                string certificatePath = Path.IsPathRooted(configPath)
                    ? configPath
                    : Path.Combine(string.IsNullOrEmpty(HostingEnvironment.ApplicationPhysicalPath)
                        ? assemblyDir
                        : HostingEnvironment.ApplicationPhysicalPath, configPath);
                return certificatePath;
            }
        }

    }
}