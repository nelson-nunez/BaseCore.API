using System;
using System.Text.Json.Serialization;

namespace BaseRest.Core.API.Common
{
    public class RefreshToken
    {
        [JsonPropertyName("username")]
        public string UserName { get; set; }
        [JsonPropertyName("tokenString")]
        public string TokenString { get; set; }
        [JsonPropertyName("expireAt")]
        public DateTime ExpireAt { get; set; }
    }
}