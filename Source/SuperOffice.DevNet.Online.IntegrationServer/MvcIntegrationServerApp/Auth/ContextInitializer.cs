using System;
using SuperOffice.Security.Principal;

namespace MvcIntegrationServerApp.Auth
{

    /// <summary>
    /// Plugin responsible for setting tenant specific information, like service url
    /// </summary>
    [ContextInitializerPlugin("OnlineTenantContextInitializer")]
    public class ContextInitializer : IContextInitializerPlugin
    {
        public void InitializeContext(string contextIdentifier)
        {
            var context = SuperOfficeAuthHelper.Context;
            if (context != null && String.Equals(contextIdentifier, context.ContextIdentifier, StringComparison.InvariantCultureIgnoreCase))
            {
                // set the default mode to remote
                SuperOffice.Configuration.ConfigFile.Services.DefaultMode = SuperOffice.CRM.Services.ServiceMode.Remote;
                // Set the tenants url.
                SuperOffice.Configuration.ConfigFile.WebServices.RemoteBaseURL = context.NetServerUrl;
            }
            
        }
    }
}