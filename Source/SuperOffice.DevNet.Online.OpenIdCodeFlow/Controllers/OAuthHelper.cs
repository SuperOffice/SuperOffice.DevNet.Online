using RestSharp;
using SuperOffice.SuperID.Client.Tokens;
using SuperOffice.SuperID.Contracts.SystemUser.V1;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;

namespace MvcTest.Controllers
{
    internal class TokensResponse
    {
        public string token_type { get; set; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string id_token { get; set; }
    }


    public class OAuthHelper
    {

        public static void GetAuthorizationCode(HttpServerUtilityBase server, HttpSessionStateBase session, string code)
        {
            GetSuperIdTokens(server, session, code, null, "authorization_code");
        }

        public static void GetRefreshToken(HttpServerUtilityBase server, HttpSessionStateBase session, string refresh_token)
        {
            GetSuperIdTokens(server, session, null, refresh_token, "refresh_token");
        }

        private static void GetSuperIdTokens(HttpServerUtilityBase server, HttpSessionStateBase session, string code, string refreshToken, string grant_type)
        {
            // Make new request from SuperId to get refresh token

            string appId = ConfigurationManager.AppSettings["SoAppId"];
            string appToken = ConfigurationManager.AppSettings["SoAppToken"];
            string appUrl = ConfigurationManager.AppSettings["SoAppUrl"];
            string url = ConfigurationManager.AppSettings["SoFederationGateway"];

            // Make the request from the server, since the secret appToken is used
            var client = new RestClient(url);
            var request = new RestRequest("common/oauth/tokens", Method.POST);
            request.AddParameter("client_id", appId);
            request.AddParameter("client_secret", appToken);
            if( code != null )
                request.AddParameter("code", code);
            if( refreshToken!= null )
                request.AddParameter("refresh_token", refreshToken);
            request.AddParameter("redirect_uri", appUrl);
            request.AddParameter("grant_type", grant_type);

            var response = client.Execute<TokensResponse>(request);
            if (response.IsSuccessful)
            {
                var tokens = response.Data;

                StoreTokensInSession(server, session, tokens);
            }
            else
            {   // Something is wrong
                throw new Exception("SuperId: Tokens request failed: " + response.ErrorMessage ?? "null");
            }
        }


        private static void StoreTokensInSession(HttpServerUtilityBase server, HttpSessionStateBase session, TokensResponse tokens)
        {
            // Validate JWT token
            var superIdToken = ValidateToken(server, tokens.id_token);
            if (superIdToken == null)
                throw new Exception("Invalid JWT token: " + tokens.id_token );

            // Valid JWT - Store tokens
            session["LoggedIn"] = tokens.access_token;
            session["RefreshToken"] = tokens.refresh_token;
            session["Expires"] = DateTime.Now.AddSeconds(tokens.expires_in);
            session["Token"] = superIdToken;
            session["NetServerUrl"] = superIdToken.NetserverUrl;
        }

        public static SuperIdToken ValidateToken(HttpServerUtilityBase server, string token)
        {
            var path = server.MapPath("~/App_Data/") + "SOD_SuperOfficeFederatedLogin.crt";

            var tokenHandler = new SuperIdTokenHandler();
            tokenHandler.JwtIssuerSigningCertificate = new X509Certificate2(path);
            tokenHandler.CertificateValidator = X509CertificateValidator.ChainTrust;
            tokenHandler.ValidIssuer = "https://sod.superoffice.com";

            return tokenHandler.ValidateToken(token, TokenType.Jwt);
        }


    }
}