using System;
using System.Collections.Generic;
using System.Configuration;

namespace MvcIntegrationServerApp.Auth
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


        public static string ApplicationId
        {
            get { return ConfigurationManager.AppSettings["ApplicationId"]; }
        }

        public static string ApplicationKeyFile
        {
            get { return ConfigurationManager.AppSettings["ApplicationKeyFile"]; }
        }
        public static string SuperOfficeFederatedLogin
        {
            get { return ConfigurationManager.AppSettings["SuperOfficeFederatedLogin"]; }
        }
    }
}
