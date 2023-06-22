using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Web;
using BaseRest.Core.Model.DTO;

namespace BaseRest.Core.API.Common
{
    public static class WebApiClientExt2
    {


        public static async Task AuthenticationJWT(this WebApiClient webApiClient, string jwt)
        {
            webApiClient.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        }

        public static async Task<bool> ValidateAccessToken(this WebApiClient webApiClient, IHttpContextAccessor _contextAccessor, NavigationManager _navigationManager,
            string prefix = "")
        {

            var loginUrl = AppConfiguration.GetConfiguration("LoginUrl");
            var user = _contextAccessor.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                _navigationManager.NavigateTo(loginUrl, true);
                return false;
            }
            var warningMinutes = Convert.ToDouble(AppConfiguration.GetConfiguration("WarningMinutes"));

            var refreshTokenEndpoint = AppConfiguration.GetConfiguration("RefreshTokenEndpoint");

            var authTokenClaimName = string.IsNullOrWhiteSpace(prefix) ? "authToken" : $"{prefix}-authToken";
            var refreshTokenClaimName = string.IsNullOrWhiteSpace(prefix) ? "refreshToken" : $"{prefix}-refreshToken";
            var expClaimName = string.IsNullOrWhiteSpace(prefix) ? "exp" : $"{prefix}-exp";

            var accessTokenExpirationClaim = user.Claims.FirstOrDefault(x => x.Type == expClaimName);
            var authTokenClaim = user.Claims.FirstOrDefault(x => x.Type == authTokenClaimName);
            var refreshTokenClaim = user.Claims.FirstOrDefault(x => x.Type == refreshTokenClaimName);
            DateTimeOffset offset = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(accessTokenExpirationClaim.Value));
            DateTime expirationDate = offset.LocalDateTime;



            if (DateTime.Now < expirationDate.AddMinutes(warningMinutes))
            {
                webApiClient.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authTokenClaim.Value);
                return true;
            }
            else
            {
                var dtoAsJson = JsonSerializer.Serialize(new { AccessToken = authTokenClaim.Value, RefreshToken = refreshTokenClaim.Value });

                var response = await webApiClient.httpClient.PostAsync(refreshTokenEndpoint, new StringContent(dtoAsJson, Encoding.UTF8, "application/json"));
                if (!response.IsSuccessStatusCode)
                {
                    _navigationManager.NavigateTo(loginUrl, true);
                    return false;
                }
                else
                {
                    var tokens = await response.Content.ReadFromJsonAsync<JwtAuthResult>();
                    var identity = user.Identity as ClaimsIdentity;

                    var claimsCopy = user.Claims.ToList();
                    foreach (var claim in claimsCopy)
                    {
                        if (claim.Type.Contains(prefix))
                            identity.RemoveClaim(claim);
                    }


                    webApiClient.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

                    identity.AddClaims(ServiceExtensions.ParseClaimsFromJwt(tokens.AccessToken, prefix));

                    identity.AddClaim(new Claim(refreshTokenClaimName, tokens.RefreshToken.TokenString));

                    return true;
                }


            }
        }


        public static async Task<SigninResponse> AuthenticationJWT(this WebApiClient webApiClient, string resource, SigninRequest credentials)
        {
            try
            {
                var loginAsJson = JsonSerializer.Serialize(credentials);
                var response = await webApiClient.httpClient.PostAsync("api/Authentication", new StringContent(loginAsJson, Encoding.UTF8, "application/json"));
                var loginResult = JsonSerializer.Deserialize<SigninResponse>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (response.IsSuccessStatusCode)
                {
                    return loginResult;
                }

                webApiClient.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.AccessToken);

                return loginResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<object> GetAsync(this WebApiClient webApiClient, string resource)
        {
            var response = await webApiClient.httpClient.GetAsync(resource);

            if (response.IsSuccessStatusCode)
                return response.Content;
            else
            {
                var errorDto = response.Content.ReadFromJsonAsync<ExceptionDTO>().Result;
                if (string.IsNullOrEmpty(errorDto.LogId))
                    throw new RemoteBusinessException(errorDto.Message);
                else
                    throw new RemoteUnknownException(errorDto.Message, $"{webApiClient.httpClient.BaseAddress.AbsoluteUri}{resource}", errorDto.LogId);
            }
        }

        public static async Task<T> GetAsync<T>(this WebApiClient webApiClient, string resource) where T : new()
        {
            var response = await webApiClient.httpClient.GetAsync(resource);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadFromJsonAsync<T>().Result;
                return result;
            }
            else
            {
                var errorDto = response.Content.ReadFromJsonAsync<ExceptionDTO>().Result;
                if (string.IsNullOrEmpty(errorDto.LogId))
                    throw new RemoteBusinessException(errorDto.Message);
                else
                    throw new RemoteUnknownException(errorDto.Message, $"{webApiClient.httpClient.BaseAddress.AbsoluteUri}{resource}", errorDto.LogId);
            }
        }


        public static async Task<dynamic> GetDynamicObjectAsync(this WebApiClient webApiClient, string resource)
        {
            var response = await webApiClient.httpClient.GetAsync(resource);

            if (response.IsSuccessStatusCode)
            {
                dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                return result;
            }
            else
            {
                var errorDto = response.Content.ReadFromJsonAsync<ExceptionDTO>().Result;
                if (string.IsNullOrEmpty(errorDto.LogId))
                    throw new RemoteBusinessException(errorDto.Message);
                else
                    throw new RemoteBusinessException(errorDto.Message, errorDto.LogId);
            }
        }

        public static async Task<dynamic> PostAndReceiveDynamicObjectAsync(this WebApiClient webApiClient, string resource, object body)
        {
            var response = await webApiClient.httpClient.PostAsJsonAsync(resource, body);

            if (response.IsSuccessStatusCode)
            {
                dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                return result;
            }
            else
            {
                var errorDto = response.Content.ReadFromJsonAsync<ExceptionDTO>().Result;
                if (string.IsNullOrEmpty(errorDto.LogId))
                    throw new RemoteBusinessException(errorDto.Message);
                else
                    throw new RemoteUnknownException(errorDto.Message, $"{webApiClient.httpClient.BaseAddress.AbsoluteUri}{resource}", errorDto.LogId);
            }
        }

        public static T Get<T>(this WebApiClient webApiClient, string resource) where T : new()
        {
            Task<HttpResponseMessage> task = Task.Run<HttpResponseMessage>(async () => await webApiClient.httpClient.GetAsync(resource));
            var response = task.Result;

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadFromJsonAsync<T>().Result;
                return result;
            }
            else
            {
                var errorDto = response.Content.ReadFromJsonAsync<ExceptionDTO>().Result;
                if (string.IsNullOrEmpty(errorDto.LogId))
                    throw new RemoteBusinessException(errorDto.Message);
                else
                    throw new RemoteUnknownException(errorDto.Message, $"{webApiClient.httpClient.BaseAddress.AbsoluteUri}{resource}", errorDto.LogId);
            }
        }

        public static async Task<T> Get<T>(this WebApiClient webApiClient, string resource, object args) where T : new()
        {
            UriBuilder builder = new UriBuilder($"{webApiClient.httpClient.BaseAddress}{resource}");
            var query = HttpUtility.ParseQueryString(builder.Query);

            PropertyInfo[] properties = args.GetType().GetProperties();
            foreach (var property in properties)
            {
                AddQueryParameterCustom(query, $"{property.Name}", property.GetValue(args));
            }

            var response = await webApiClient.httpClient.GetAsync(builder.ToString());

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var data = await response.Content.ReadFromJsonAsync<T>();
                return data;
            }
            else
            {
                var errorDto = response.Content.ReadFromJsonAsync<ExceptionDTO>().Result;
                if (string.IsNullOrEmpty(errorDto.LogId))
                    throw new RemoteBusinessException(errorDto.Message);
                else
                    throw new RemoteUnknownException(errorDto.Message, $"{webApiClient.httpClient.BaseAddress.AbsoluteUri}{resource}", errorDto.LogId);
            }
        }

        public static void AddQueryParameterCustom(NameValueCollection query, string name, object value)
        {
            string stringValue = string.Empty;
            if (value != null)
            {
                var typeName = value.GetType().Name;
                var nullType = Nullable.GetUnderlyingType(value.GetType());
                if (nullType != null)
                    typeName = nullType.Name;

                switch (typeName)
                {
                    case "DateTime":
                        var dateTime = Convert.ToDateTime(value);
                        stringValue = dateTime.ToString("yyyy-MM-dd");
                        break;
                    default:
                        stringValue = value?.ToString();
                        break;
                }
            }

            query[name] = stringValue;
        }

        public static async Task<object> PostAsync(this WebApiClient webApiClient, string resource, object dto)
        {
            var dtoAsJson = JsonSerializer.Serialize(dto);
            var response = await webApiClient.httpClient.PostAsync(resource, new StringContent(dtoAsJson, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
                return response.Content;
            else
            {
                var errorDto = response.Content.ReadFromJsonAsync<ExceptionDTO>().Result;
                if (string.IsNullOrEmpty(errorDto.LogId))
                    throw new RemoteBusinessException(errorDto.Message);
                else
                    throw new RemoteUnknownException(errorDto.Message, $"{webApiClient.httpClient.BaseAddress.AbsoluteUri}{resource}", errorDto.LogId);
            }
        }

        public static async Task<T> PostAsync<T>(this WebApiClient webApiClient, string resource, object dto) where T : new()
        {
            var dtoAsJson = JsonSerializer.Serialize(dto);
            var response = await webApiClient.httpClient.PostAsync(resource, new StringContent(dtoAsJson, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<T>();
            else
            {
                var errorDto = response.Content.ReadFromJsonAsync<ExceptionDTO>().Result;
                if (string.IsNullOrEmpty(errorDto.LogId))
                    throw new RemoteBusinessException(errorDto.Message);
                else
                    throw new RemoteUnknownException(errorDto.Message, $"{webApiClient.httpClient.BaseAddress.AbsoluteUri}{resource}", errorDto.LogId);
            }
        }

        public static async Task<T> PutAsync<T>(this WebApiClient webApiClient, string resource, object dto) where T : new()
        {
            var dtoAsJson = JsonSerializer.Serialize(dto);
            var response = await webApiClient.httpClient.PutAsync(resource, new StringContent(dtoAsJson, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<T>();
            else
            {
                var errorDto = response.Content.ReadFromJsonAsync<ExceptionDTO>().Result;
                if (string.IsNullOrEmpty(errorDto.LogId))
                    throw new RemoteBusinessException(errorDto.Message);
                else
                    throw new RemoteUnknownException(errorDto.Message, $"{webApiClient.httpClient.BaseAddress.AbsoluteUri}{resource}", errorDto.LogId);
            }
        }

        public static async Task<T> PutAsync<T>(this WebApiClient webApiClient, string resource) where T : new()
        {
            var response = await webApiClient.httpClient.PutAsync(resource, null);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<T>();
            else
            {
                var errorDto = response.Content.ReadFromJsonAsync<ExceptionDTO>().Result;
                if (string.IsNullOrEmpty(errorDto.LogId))
                    throw new RemoteBusinessException(errorDto.Message);
                else
                    throw new RemoteUnknownException(errorDto.Message, $"{webApiClient.httpClient.BaseAddress.AbsoluteUri}{resource}", errorDto.LogId);
            }
        }

        public static async Task<T> PatchAsync<T>(this WebApiClient webApiClient, string resource, object dto) where T : new()
        {
            var dtoAsJson = JsonSerializer.Serialize(dto);
            var response = await webApiClient.httpClient.PatchAsync(resource, new StringContent(dtoAsJson, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<T>();
            else
            {
                var errorDto = response.Content.ReadFromJsonAsync<ExceptionDTO>().Result;
                if (string.IsNullOrEmpty(errorDto.LogId))
                    throw new RemoteBusinessException(errorDto.Message);
                else
                    throw new RemoteUnknownException(errorDto.Message, $"{webApiClient.httpClient.BaseAddress.AbsoluteUri}{resource}", errorDto.LogId);
            }
        }

        public static async Task<T> DeleteAsync<T>(this WebApiClient webApiClient, string resource) where T : new()
        {
            var response = await webApiClient.httpClient.DeleteAsync(resource);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<T>();
            else
            {
                var errorDto = response.Content.ReadFromJsonAsync<ExceptionDTO>().Result;
                if (string.IsNullOrEmpty(errorDto.LogId))
                    throw new RemoteBusinessException(errorDto.Message);
                else
                    throw new RemoteUnknownException(errorDto.Message, $"{webApiClient.httpClient.BaseAddress.AbsoluteUri}{resource}", errorDto.LogId);
            }
        }

    }
}
