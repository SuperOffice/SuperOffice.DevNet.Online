using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Web;
using SuperOffice.Security.Principal;


namespace MvcIntegrationServerApp.Auth
{

    /// <summary>
    /// Plugin responsible for resolving the tenant.
    /// </summary>
    [ContextResolverPlugin("OnlineTenantContextResolver", 100)]
    public class ContextResolver : IContextResolverPlugin
    {
        /// <summary>
        /// Resolve tenant based on information passed back from teh login page.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="contextIdentifier"></param>
        /// <returns></returns>
        bool IContextResolverPlugin.TryResolveContext(System.IdentityModel.Tokens.SecurityToken[] tokens, out string contextIdentifier)
        {
            var context = SuperOfficeAuthHelper.Context;
            if (context != null)
            {
                contextIdentifier = context.ContextIdentifier;
                return true;
            }
            else
            {
                contextIdentifier = string.Empty;
                return false;
            }
        }


        public void AppendSecurityToken(IList<SecurityToken> tokens)
        {
        }
    }
}