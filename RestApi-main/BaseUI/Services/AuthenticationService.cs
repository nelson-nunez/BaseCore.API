using BaseRest.Core.API.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace BaseUI.Services
{
    public class AuthenticationService
    {
        private readonly WebApiClient baseApiClient;
        private IHttpContextAccessor contextAccessor;

        public AuthenticationService(WebApiClient baseApiClient, IHttpContextAccessor contextAccessor)
        {
            this.baseApiClient = baseApiClient;
            this.contextAccessor = contextAccessor;
        }

        public async Task<JwtAuthResult> Authenticate(string username, string password)
        {
            try
            {
                var authenticationResult = await baseApiClient.PostAsync<JwtAuthResult>($"Authentication/Authenticate", new SigninRequest
                {
                    UserName = username,
                    Password = password,
                });
                return authenticationResult;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

