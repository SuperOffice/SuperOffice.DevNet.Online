using MvcIntegrationServerApp.Auth;
using SuperOffice.Online.IntegrationService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web.Hosting;
using SuperOffice.CRM;
using SuperOffice;
using SuperOffice.Security.Principal;
using SuperOffice.SuperID.Client.Tokens;
using System.IdentityModel.Selectors;

namespace MvcIntegrationServerApp.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "QuoteConnector" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select QuoteConnector.svc or QuoteConnector.svc.cs at the Solution Explorer and start debugging.
    public class QuoteConnector : OnlineQuoteConnector<SuperOffice.JsonQuoteConnector>
    {
        public QuoteConnector() : base
            (
                  // Application Identifier
                 ConfigManager.ApplicationId,
                  // Application Private key file
                  GetPrivateKey(ConfigManager.ApplicationKeyFile)
            )
        {
        }

        public string TemplateFolder
        {
            get
            {
                return HostingEnvironment.MapPath("~/App_Data/Template/");
            }
        }

        protected override TResponse Execute<TRequest, TResponse>(TRequest request, Action<IQuoteConnector, TResponse> action)
        {
            using (SoDatabaseContext.EnterDatabaseContext(request.ContextIdentifier))
            {
                var systemUserToken = SystemUserManager.GetSystemUserToken(request.ContextIdentifier);
                SuperOffice.Configuration.ConfigFile.WebServices.RemoteBaseURL = systemUserToken.NetserverUrl;

                using (SoSession session = SoSession.Authenticate(new SoCredentials() { Ticket = systemUserToken.Ticket }))
                {
                    return base.Execute<TRequest, TResponse>(request, action);
                }
            }
        }

        protected override void InitializeSuperIdTokenHandler(SuperIdTokenHandler tokenHandler)
        {
            base.InitializeSuperIdTokenHandler(tokenHandler);

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

        }



        protected override SuperOffice.JsonQuoteConnector GetInnerTypedQuoteConnector<TRequest>(TRequest request)
        {
            var inner = base.GetInnerTypedQuoteConnector(request);


            var folder = ConfigurationManager.AppSettings["DataRoot"];

            if (String.IsNullOrWhiteSpace(folder))
                folder = HostingEnvironment.MapPath("~/App_Data");

            folder = Path.Combine(folder, request.ContextIdentifier);

            if (!Directory.Exists(folder))
            {
                DirectoryCopy(TemplateFolder, folder, false);
            }

            inner.FolderName = folder;

            return inner;
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
