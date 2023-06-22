using BaseRest.Core.Model.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace BaseRest.Core.API.Common
{
    public class WebApiClient
    {
        public readonly HttpClient httpClient;
        private readonly IHttpContextAccessor contextAccessor;

        public WebApiClient(HttpClient httpClient, IHttpContextAccessor contextAccessor)
        {

            this.httpClient = httpClient;
            this.contextAccessor = contextAccessor;

            var claimToken = contextAccessor.HttpContext.User.Claims.SingleOrDefault(x => x.Type == "authToken");
            if (claimToken != null)
            {
                var jwt = claimToken.Value;
                this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            }

        }

        public string BaseAddress
        {
            set { httpClient.BaseAddress = new Uri(value); }
        }

        public async Task AuthenticationJWT(string jwt)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        }
    
        public async Task<SigninResponse> AuthenticationJWT(string resource, SigninRequest credentials)
        {
            try
            {
                var loginAsJson = JsonSerializer.Serialize(credentials);
                var response = await httpClient.PostAsync("api/Authentication", new StringContent(loginAsJson, Encoding.UTF8, "application/json"));
                var loginResult = JsonSerializer.Deserialize<SigninResponse>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (response.IsSuccessStatusCode)
                {
                    return loginResult;
                }

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.AccessToken);

                return loginResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region GETS
    
        public async Task<object> GetAsync(string resource)
        {
            var response = await httpClient.GetAsync(resource);

            if (response.IsSuccessStatusCode)
                return response.Content;
            else
            {
                var errorDto = response.Content.ReadFromJsonAsync<ExceptionDTO>().Result;
                if (string.IsNullOrEmpty(errorDto.LogId))
                    throw new RemoteBusinessException(errorDto.Message);
                else
                    throw new RemoteUnknownException(errorDto.Message, $"{httpClient.BaseAddress.AbsoluteUri}{resource}", errorDto.LogId);
            }
        }

        public async Task<T> GetAsync<T>(string resource) where T : new()
        {
            var response = await httpClient.GetAsync(resource);

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
                    throw new RemoteUnknownException(errorDto.Message, $"{httpClient.BaseAddress.AbsoluteUri}{resource}", errorDto.LogId);
            }
        }

        public T Get<T>(string resource) where T : new()
        {
            Task<HttpResponseMessage> task = Task.Run<HttpResponseMessage>(async () => await httpClient.GetAsync(resource));
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
                    throw new RemoteUnknownException(errorDto.Message, $"{httpClient.BaseAddress.AbsoluteUri}{resource}", errorDto.LogId);
            }
        }

        public async Task<T> Get<T>(string resource, object args) where T : new()
        {
            UriBuilder builder = new UriBuilder($"{httpClient.BaseAddress}{resource}");
            var query = HttpUtility.ParseQueryString(builder.Query);

            PropertyInfo[] properties = args.GetType().GetProperties();
            foreach (var property in properties)
            {
                AddQueryParameterCustom(query, $"{property.Name}", property.GetValue(args));
            }

            builder.Query = query.ToString();

            var response = await httpClient.GetAsync(builder.ToString());

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
                    throw new RemoteUnknownException(errorDto.Message, $"{httpClient.BaseAddress.AbsoluteUri}{resource}", errorDto.LogId);
            }
        }

        public async Task<dynamic> GetDynamicObjectAsync(string resource)
        {
            var response = await httpClient.GetAsync(resource);

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
        
        #endregion

        #region POST

        public async Task<dynamic> PostAndReceiveDynamicObjectAsync(string resource, object body)
        {
            var response = await httpClient.PostAsJsonAsync(resource, body);

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
                    throw new RemoteUnknownException(errorDto.Message, $"{httpClient.BaseAddress.AbsoluteUri}{resource}", errorDto.LogId);
            }
        }

        public async Task<object> PostAsync(string resource, object dto)
        {
            var dtoAsJson = JsonSerializer.Serialize(dto);
            var response = await httpClient.PostAsync(resource, new StringContent(dtoAsJson, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
                return response.Content;
            else
            {
                var errorDto = response.Content.ReadFromJsonAsync<ExceptionDTO>().Result;
                if (string.IsNullOrEmpty(errorDto.LogId))
                    throw new RemoteBusinessException(errorDto.Message);
                else
                    throw new RemoteUnknownException(errorDto.Message, $"{httpClient.BaseAddress.AbsoluteUri}{resource}", errorDto.LogId);
            }
        }

        public async Task<T> PostAsync<T>(string resource, object dto) where T : new()
        {
            var dtoAsJson = JsonSerializer.Serialize(dto);
            var response = await httpClient.PostAsync(resource, new StringContent(dtoAsJson, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<T>();
            else
            {
                var errorDto = response.Content.ReadFromJsonAsync<ExceptionDTO>().Result;
                if (string.IsNullOrEmpty(errorDto.LogId))
                    throw new RemoteBusinessException(errorDto.Message);
                else
                    throw new RemoteUnknownException(errorDto.Message, $"{httpClient.BaseAddress.AbsoluteUri}{resource}", errorDto.LogId);
            }
        }

        #endregion

        #region PUT

        public async Task<T> PutAsync<T>(string resource, object dto) where T : new()
        {
            var dtoAsJson = JsonSerializer.Serialize(dto);
            var response = await httpClient.PutAsync(resource, new StringContent(dtoAsJson, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<T>();
            else
            {
                var errorDto = response.Content.ReadFromJsonAsync<ExceptionDTO>().Result;
                if (string.IsNullOrEmpty(errorDto.LogId))
                    throw new RemoteBusinessException(errorDto.Message);
                else
                    throw new RemoteUnknownException(errorDto.Message, $"{httpClient.BaseAddress.AbsoluteUri}{resource}", errorDto.LogId);
            }
        }

        public async Task<T> PutAsync<T>(string resource) where T : new()
        {
            var response = await httpClient.PutAsync(resource, null);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<T>();
            else
            {
                var errorDto = response.Content.ReadFromJsonAsync<ExceptionDTO>().Result;
                if (string.IsNullOrEmpty(errorDto.LogId))
                    throw new RemoteBusinessException(errorDto.Message);
                else
                    throw new RemoteUnknownException(errorDto.Message, $"{httpClient.BaseAddress.AbsoluteUri}{resource}", errorDto.LogId);
            }
        }

        public async Task<T> PatchAsync<T>(string resource, object dto) where T : new()
        {
            var dtoAsJson = JsonSerializer.Serialize(dto);
            var response = await httpClient.PatchAsync(resource, new StringContent(dtoAsJson, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<T>();
            else
            {
                var errorDto = response.Content.ReadFromJsonAsync<ExceptionDTO>().Result;
                if (string.IsNullOrEmpty(errorDto.LogId))
                    throw new RemoteBusinessException(errorDto.Message);
                else
                    throw new RemoteUnknownException(errorDto.Message, $"{httpClient.BaseAddress.AbsoluteUri}{resource}", errorDto.LogId);
            }
        }

        #endregion

        #region DELETE
        
        public async Task<T> DeleteAsync<T>(string resource) where T : new()
        {
            var response = await httpClient.DeleteAsync(resource);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<T>();
            else
            {
                var errorDto = response.Content.ReadFromJsonAsync<ExceptionDTO>().Result;
                if (string.IsNullOrEmpty(errorDto.LogId))
                    throw new RemoteBusinessException(errorDto.Message);
                else
                    throw new RemoteUnknownException(errorDto.Message, $"{httpClient.BaseAddress.AbsoluteUri}{resource}", errorDto.LogId);
            }
        }

        #endregion
     
        public void AddQueryParameterCustom(NameValueCollection query, string name, object value)
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

    }
}