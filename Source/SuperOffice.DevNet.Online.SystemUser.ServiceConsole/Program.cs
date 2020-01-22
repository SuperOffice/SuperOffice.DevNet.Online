using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SuperOffice;
using SuperOffice.Configuration;
using SuperOffice.CRM.ArchiveLists;
using SuperOffice.CRM.Services;
using SuperOffice.Security.Principal;
using SuperOffice.SuperID.Client.Tokens;
using SuperOffice.SuperID.Contracts.SystemUser.V1;

namespace SuperOffice.DevNet.Online.SystemUser.ServiceConsole
{
    class Program
    {
        private static EventWaitHandle waitHandle = null;

        static void Main(string[] args)
        {
            SyncCustomers();
            Log("Done processing. Press any key to exit!");
            var result = Console.ReadLine();
        }

        private static void SyncCustomers()
        {
            var customerDataSource = new SuperOffice.DevNet.Online.SystemUser.PartnerDBLibrary.Models.CustomerDataSource();

            foreach (var customer in customerDataSource.Customers)
            {
                Log("Getting System User Token.");

                var token = GetSystemUserToken(customer.SystemUserToken, customer.ContextIdentifier);

                if(token != null)
                {
                    // Enter database context for the customer (enter the right multi-tenant context)
                    using (var context = SoDatabaseContext.EnterDatabaseContext(token.ContextIdentifier))
                    {
                        // set appriiriate url for the customer tenant
                        ConfigFile.WebServices.RemoteBaseURL = token.NetserverUrl;

                        try
                        {
                            // Log in as the system user
                            using (var session = SoSession.Authenticate(new SoCredentials(token.Ticket)))
                            {

                                //var listHelper = new SuperOffice.DevNet.Online.Provisioning.ListHelper();
                                //listHelper.CreateSaleSourceListItem("NewSaleSource", "NewSaleSourceToolTip");

                                //// Do work as the system user
                                Log("Logged on to context {0} as {1}", token.ContextIdentifier, SoContext.CurrentPrincipal == null ? "Unknown" : SoContext.CurrentPrincipal.Associate);
                            }
                        }
                        catch (Exception ex)
                        {
                            while (ex.InnerException != null)
                            {
                                ex = ex.InnerException;
                            }

                            Log("Exception during authentication for customer {0}: {1}", token.ContextIdentifier, ex.Message);
                        }
                    }
                }
                else
                {
                    Log("Unable to get token for customer {0}.", customer.ContextIdentifier);
                }

            }
        }


        /// <summary>
        /// Get and validate claims for the system user from the SuperID web service
        /// </summary>
        /// <param name="userToken">System user token, not yet signed</param>
        /// <param name="contextIdentifier">Context identifier of the customer</param>
        /// <returns>Token with claims</returns>
        private static SuperIdToken GetSystemUserToken(string userToken, string contextIdentifier)
        {
            var tokenType = SuperOffice.SuperID.Contracts.SystemUser.V1.TokenType.Jwt;

            var systemToken = new SystemToken(userToken);

            // Get certificate
            var certificatePath = ConfigurationManager.AppSettings["SystemTokenCertificatePath"];

            // sign the system user token
            var signedSystemToken = systemToken.Sign(File.ReadAllText(certificatePath));

            // Call the web service to exchange signed system user token with claims for the system user
            var federationGateway = ConfigurationManager.AppSettings["SoFederationGateway"];
            var returnedToken = systemToken.AuthenticateWithSignedSystemToken(federationGateway, signedSystemToken,
                ConfigFile.Services.ApplicationToken, contextIdentifier, tokenType);

            if(returnedToken != null)
            {
                // Validate and return SuperId token for the system user
                var tokenHandler = new SuperIdTokenHandler();

                var certificateResolverPath = AppDomain.CurrentDomain.BaseDirectory + "Certificates";

                if(tokenType == SuperID.Contracts.SystemUser.V1.TokenType.Saml)
                {
                    tokenHandler.CertificateValidator = System.IdentityModel.Selectors.X509CertificateValidator.None;
                    tokenHandler.IssuerTokenResolver = new SuperOffice.SuperID.Client.Tokens.CertificateFileCertificateStoreTokenResolver(certificateResolverPath);
                }
                else
                {
                    tokenHandler.JwtIssuerSigningCertificate =
                    new System.Security.Cryptography.X509Certificates.X509Certificate2(
                        certificateResolverPath + "\\SODSuperOfficeFederatedLogin.crt");
                }

                tokenHandler.ValidateAudience = false;

                return tokenHandler.ValidateToken(returnedToken, tokenType);
            }

            return null;
        }

        private static void Log(string message)
        {
            Console.WriteLine(DateTime.Now + ": " + message);
        }

        private static void Log(string format, params object[] values)
        {
            Console.WriteLine(DateTime.Now + ": " + format, values);
        }

    }
}
