using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace BaseRest.Core.API.Common
{
    public static class ServiceExtensions
    {
        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt, string prefix = "")
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            keyValuePairs.Add("authToken", jwt);
            if (string.IsNullOrWhiteSpace(prefix))
                return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
            else
                return keyValuePairs.Select(kvp => new Claim($"{prefix}-{kvp.Key}", kvp.Value.ToString()));
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
