using System.Configuration;
using System.Web.Configuration;

namespace SuperOffice.DevNet.Online.Login
{
    public static class ConfigManager
    {
        /// <summary>
        /// Get the name of cookie used for authentication
        /// </summary>
        public static string SoAuthCookie
        {
            get { return ConfigurationManager.AppSettings["SoAuthCookie"]; }
        }

	    /// <summary>
	    /// Get the cookie name used for storing session ids
	    /// </summary>
		private static string _soSessionCookieName = "PartnerDEMO";
        public static string SoSessionCookieName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_soSessionCookieName))
                {
                    var sessionState = WebConfigurationManager.GetSection("system.web/sessionState") as SessionStateSection;
                    _soSessionCookieName = sessionState.CookieName;
                }

                return _soSessionCookieName; //; 
			}
	        set { _soSessionCookieName = value; }

        }

        /// <summary>
        /// Get the url for the federation gateway
        /// </summary>
        public static string SoFederationGateway
        {
            get { return ConfigurationManager.AppSettings["SoFederationGateway"]; }
        }
    }
}