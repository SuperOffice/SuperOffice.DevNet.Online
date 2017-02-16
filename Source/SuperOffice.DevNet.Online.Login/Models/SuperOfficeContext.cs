using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SuperOffice.Util;

namespace SuperOffice.DevNet.Online.Login
{
    /// <summary>
    /// DTO for storing SuperOffice Context.   
    /// </summary>
    [Serializable]
    public class SuperOfficeContext
    {
        public int AssociateId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string Ticket { get; set; }
        public string NetServerUrl { get; set; }
        public string Email { get; set; }
        public string ContextIdentifier { get; set; }
        public string SystemToken { get; set; }
        public string CustomerKey { get; internal set; }
        public bool IsOnSiteCustomer { get; internal set; }
        public object WebClientUrl { get; internal set; }
        public bool? IsOnlineCustomer { get; internal set; }

        public string GetUsersInstallationUrl()
		{
			string url = string.Empty;

			if( !NetServerUrl.IsNullOrEmpty() )
			{
				// Remove the /Remote/services?? from the url:

				url = NetServerUrl.Substring( 0, NetServerUrl.IndexOf( "/remote/", StringComparison.InvariantCultureIgnoreCase ) );
			}

			return url;
		}


    }
}