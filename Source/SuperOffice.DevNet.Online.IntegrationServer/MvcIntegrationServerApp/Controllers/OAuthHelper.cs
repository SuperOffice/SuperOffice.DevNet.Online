using AngleSharp.Io;
using IdentityModel.Client;
using Microsoft.IdentityModel.Tokens;
using MvcIntegrationServerApp.Auth;
using Newtonsoft.Json.Linq;
using SuperOffice.SuperID.Client.Tokens;
using SuperOffice.SuperID.Contracts.SystemUser.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.ServiceModel;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Security;

namespace MvcIntegrationServerApp.Controllers
{
    public class OAuthHelper
    {
        public static async Task InitializeOpenIdConnectEndpointsAsync()
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    // Construct the discovery endpoint URL
                    string discoveryUrl = $"https://{Global.Settings.Auth.Environment}.superoffice.com/login/.well-known/openid-configuration";

                    // Fetch and parse the discovery document
                    var response = await httpClient.GetStringAsync(discoveryUrl);
                    using (JsonDocument document = JsonDocument.Parse(response))
                    {
                        JsonElement root = document.RootElement;

                        // Extract and set the required endpoints, throwing an exception if not found
                        string authEndpoint = root.GetProperty("authorization_endpoint").GetString();
                        Global.Settings.OpenIdConnectConfiguration.AuthEndpoint = root.GetProperty("authorization_endpoint").GetString()
                            ?? throw new InvalidOperationException("Authorization endpoint is null or missing.");
                        Global.Settings.OpenIdConnectConfiguration.TokenEndpoint = root.GetProperty("token_endpoint").GetString()
                            ?? throw new InvalidOperationException("Token endpoint is null or missing.");
                        Global.Settings.OpenIdConnectConfiguration.JwksUri = root.GetProperty("jwks_uri").GetString()
                            ?? throw new InvalidOperationException("JWKS URI is null or missing.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Handle HTTP errors
                    Console.WriteLine($"HTTP request error: {ex.Message}");
                    throw;
                }
                catch (JsonException ex)
                {
                    // Handle JSON parsing errors
                    Console.WriteLine($"JSON parsing error: {ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    // Handle any other general errors
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                    throw;
                }
            }
        }

        private static async Task GetSuperIdTokens(HttpSessionStateBase session, string code, string refreshToken, string grantType)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    // Prepare the content for the token request
                    var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_id", Global.Settings.Auth.ClientId),
                new KeyValuePair<string, string>("client_secret", Global.Settings.Auth.ClientSecret),
                new KeyValuePair<string, string>("grant_type", grantType),
                new KeyValuePair<string, string>("redirect_uri", Global.Settings.Auth.RedirectUri)
            };

                    // Conditionally add parameters based on their presence
                    if (!string.IsNullOrEmpty(code))
                    {
                        parameters.Add(new KeyValuePair<string, string>("code", code));
                    }
                    if (!string.IsNullOrEmpty(refreshToken))
                    {
                        parameters.Add(new KeyValuePair<string, string>("refresh_token", refreshToken));
                    }

                    // Create the request content
                    var requestContent = new FormUrlEncodedContent(parameters);

                    // Send the POST request to the token endpoint
                    var response = await httpClient.PostAsync(Global.Settings.OpenIdConnectConfiguration.TokenEndpoint, requestContent);

                    // Ensure the response is successful and handle potential issues
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorDetails = await response.Content.ReadAsStringAsync();
                        throw new HttpRequestException($"Failed to get token. Status code: {response.StatusCode}. Details: {errorDetails}");
                    }

                    // Read and deserialize the response content
                    var tokenJson = await response.Content.ReadAsStringAsync();
                    var tokenData = JsonSerializer.Deserialize<Models.TokenResponse>(tokenJson);

                    TokenValidationResult result = ValidateSuperOfficeToken(tokenData.IdToken);

                    if (result.IsValid)
                    {
                        // TODO: get the systemuserToken from the id_token and stick it a file together with tenant/custId. Then run the SystemUser flow to get an actuall access_token and/or refresh_token we can store.
                        //session["AccessToken"] = tokenData.AccessToken;

                        var context = new SuperOfficeContext
                        {
                            Email = result.Claims.FirstOrDefault(c => c.Key.Contains("http://schemes.superoffice.net/identity/email")).Value.ToString(),
                            ContextIdentifier = result.Claims.FirstOrDefault(c => c.Key.Contains("http://schemes.superoffice.net/identity/ctx")).Value.ToString(),
                            NetServerUrl = result.Claims.FirstOrDefault(c => c.Key.Contains("http://schemes.superoffice.net/identity/netserver_url")).Value.ToString(),
                            SystemToken = result.Claims.FirstOrDefault(c => c.Key.Contains("http://schemes.superoffice.net/identity/system_token")).Value.ToString(),
                        };

                        SuperOfficeAuthHelper.Context = context;

                        // Use forms authentication - this is optional
                        var soFormsTicket = new FormsAuthenticationTicket(result.Claims.FirstOrDefault(c => c.Key.Contains("http://schemes.superoffice.net/identity/email")).Value.ToString(), false, 3600);
                        var soFormsTicketEncrypted = FormsAuthentication.Encrypt(soFormsTicket);

                        var httpContext = HttpContext.Current;
                        httpContext.Session[ConfigManager.SoAuthCookie] = soFormsTicketEncrypted;
                        httpContext.Response.Cookies.Add(new HttpCookie(ConfigManager.SoAuthCookie, soFormsTicketEncrypted));
                    }
                    else
                    {
                        throw new InvalidOperationException("Token response did not contain a valid access token.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Log HTTP request errors
                    Console.WriteLine($"HTTP request error: {ex.Message}");
                }
                catch (JsonException ex)
                {
                    // Log JSON parsing errors
                    Console.WriteLine($"JSON parsing error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Log other unexpected errors
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                }
            }
        }

        public static async Task GetAuthorizationCode(HttpSessionStateBase session, string code)
        {
            // TODO Add code_challenge to this request!
            await GetSuperIdTokens(session,code, null, "authorization_code");
        }

        public static async Task GetRefreshToken(HttpSessionStateBase session, string refresh_token)
        {
            await GetSuperIdTokens(session, null, refresh_token, "refresh_token");
        }

        public static string CreateAuthorizeUrl(HttpSessionStateBase session, string acrValues)
        {
            // TODO: Add code_challenge to this request!
    
            // Base URL for the authorization endpoint
            var url = new UriBuilder(Global.Settings.OpenIdConnectConfiguration.AuthEndpoint);
            var query = HttpUtility.ParseQueryString(string.Empty);

            // Generate state and nonce values
            string state = Guid.NewGuid().ToString();

            // Store them in session or a secure cookie
            session["state"] = state;

            // Add the required parameters
            query["client_id"] = Global.Settings.Auth.ClientId;
            query["redirect_uri"] = Global.Settings.Auth.RedirectUri;
            query["response_type"] = "code"; // For authorization code flow
            query["scope"] = "openid";
            query["state"] = state; // Optional but recommended for CSRF protection

            if (!string.IsNullOrEmpty(acrValues))
            {
                query["acr_values"] = "tenant:" + acrValues;
            }

            // Construct the final URL with query parameters
            url.Query = query.ToString();

            return url.ToString();
        }

        public static Microsoft.IdentityModel.Tokens.TokenValidationResult ValidateSuperOfficeToken(string token)
        {
            var securityTokenHandler =
                new Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler();

            string issuer;
            string audience;

            // extract the ValidAudience claim value (database serial number).
            var securityToken = securityTokenHandler.ReadJsonWebToken(token);

            // get the audience from the token
            if (!securityToken.TryGetPayloadValue<string>("aud", out audience))
            {
                throw new Microsoft.IdentityModel.Tokens.SecurityTokenException(
                    "Unable to read ValidAudience from token.");
            }

            // get the issuer from the token
            if (!securityToken.TryGetPayloadValue<string>("iss", out issuer))
            {
                throw new Microsoft.IdentityModel.Tokens.SecurityTokenException(
                    "Unable to read ValidAudience from token.");
            }

            var validationParameters =
                new Microsoft.IdentityModel.Tokens.TokenValidationParameters();
            validationParameters.ValidAudience = audience;
            validationParameters.ValidIssuer = issuer;

            validationParameters.IssuerSigningKeys = GetJsonWebKeys();

            var result = securityTokenHandler.ValidateToken(token, validationParameters);

            if (result.Exception != null || !result.IsValid)
            {
                throw new Microsoft.IdentityModel.Tokens.SecurityTokenValidationException(
                    "Failed to validate the token", result.Exception);
            }
            return result;
        }

        private static IList<JsonWebKey> GetJsonWebKeys()
        {
            // example only... needs exception handing...!!!
            var client = new HttpClient();
            var jwksContent = client.GetStringAsync(Global.Settings.OpenIdConnectConfiguration.JwksUri);
            return JsonWebKeySet.Create(jwksContent.Result).Keys;
        }
    }
}