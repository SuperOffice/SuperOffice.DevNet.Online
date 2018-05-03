using MvcIntegrationServerApp.Models;
using SuperOffice.Configuration;
using SuperOffice.SuperID.Client.Tokens;
using SuperOffice.SuperID.Contracts.SystemUser.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace MvcIntegrationServerApp.Auth
{
    public class SystemUserManager
    {
        class CachedToken
        {
            public DateTime CacheTime { get; set; }
            public SuperIdToken Token { get; set; }
        }
        static Dictionary<string, CachedToken> _cachedTokens = new Dictionary<string, CachedToken>(StringComparer.InvariantCultureIgnoreCase);

        public static void ClearCachedItem(string contextIdentifier)
        {
            lock (_cachedTokens)
                _cachedTokens.Remove(contextIdentifier);
        }

        public static SuperIdToken GetSystemUserToken(string contextIdentifier)
        {
            lock (_cachedTokens)
            {
                CachedToken cachedToken;
                if (_cachedTokens.TryGetValue(contextIdentifier, out cachedToken))
                {
                    if (cachedToken.CacheTime.AddMinutes(45) > DateTime.Now)
                        return cachedToken.Token;

                }            
                using (var db = new AppDB())
                {
                    var sysUserToken = db.Customers
                        .Where(c => c.ContextIdentifier == contextIdentifier)
                        .Select(c => c.SystemUserToken)
                        .FirstOrDefault();
                    var token = GetSystemUserToken(sysUserToken, contextIdentifier);

                    _cachedTokens[contextIdentifier] = new CachedToken
                    {
                        CacheTime = DateTime.Now,
                        Token = token,
                    };

                    return token;
                }
            }
        }

        private static SuperIdToken GetSystemUserToken(string systemTokenString, string contextIdentifier)
        {
            // Grab hold of the system user token
            var systemToken = new SystemToken(systemTokenString);

            // Get certificate
            var certificatePath = ConfigManager.ApplicationKeyFile;

            if (!Path.IsPathRooted(certificatePath))
                certificatePath = Path.Combine(HostingEnvironment.MapPath(@"~"), certificatePath);

            // sign the system user token
            var signedSystemToken = systemToken.Sign(privateKey: File.ReadAllText(certificatePath));

            // Call the web service to exchange signed system user token with claims for the system user
            var federationGateway = ConfigManager.SoFederationGateway;
            var returnedToken = systemToken.AuthenticateWithSignedSystemToken(federationGateway, signedSystemToken,
                ConfigFile.Services.ApplicationToken, contextIdentifier, TokenType.Jwt);

            // Validate SuperId token for the system user
            var systemUserTokenHandler = new SuperIdTokenHandler();
            var systemUserToken = systemUserTokenHandler.ValidateToken(returnedToken, TokenType.Jwt);
            return systemUserToken;
        }
    }
}
