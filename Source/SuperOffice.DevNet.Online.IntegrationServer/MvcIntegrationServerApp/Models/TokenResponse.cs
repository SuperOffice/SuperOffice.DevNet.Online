using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcIntegrationServerApp.Models
{
    using System.Text.Json.Serialization;

    public class TokenResponse
    {
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("id_token")]
        public string IdToken { get; set; }
    }

}