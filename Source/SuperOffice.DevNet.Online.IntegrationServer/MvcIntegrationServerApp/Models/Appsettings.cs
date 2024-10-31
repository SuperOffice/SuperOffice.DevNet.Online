using SuperOffice.Online.Core.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcIntegrationServerApp.Models
{
    public class AppSettings
    {
        public AuthSettings Auth { get; set; }
        public OpenIdConnectConfiguration OpenIdConnectConfiguration { get; set; } = new OpenIdConnectConfiguration();
        public string AppName { get; set; }
    }

    public class OpenIdConnectConfiguration
    {
        public string AuthEndpoint { get; set; }
        public string TokenEndpoint { get; set; }

        public string JwksUri { get; set; }
    }
    public class AuthSettings
    {
        
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public string Environment {  get; set; }
        public string RedirectUri { get; set; }
    }
}