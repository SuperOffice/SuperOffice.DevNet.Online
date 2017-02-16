using System;
using SuperOffice.Security.Principal;
using System.Configuration;

namespace SuperOffice.DevNet.Online.Login
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
                // Set the tenants url.
                SuperOffice.Configuration.ConfigFile.WebServices.RemoteBaseURL = context.NetServerUrl;

                //Add more application specific modifications
                //...
            }

        }
    }

    ///// <summary>
    ///// Plugin responsible for setting tenant specific information, like service url
    ///// </summary>
    //[ContextInitializerPlugin("WinOnlineTenantContextInitializer")]
    //public class WinContextInitializer : IContextInitializerPlugin
    //{
    //    public void InitializeContext(string contextIdentifier)
    //    {
    //        var context = SuperOfficeAuthHelper.WinContext;
    //        if (context != null && String.Equals(contextIdentifier, context.ContextIdentifier, StringComparison.InvariantCultureIgnoreCase))
    //        {
    //            // Set the tenants url.
    //            SuperOffice.Configuration.ConfigFile.WebServices.RemoteBaseURL = context.NetServerUrl;
    //        }

    //    }
    //}
}