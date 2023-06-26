using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseRest.Core.API.Common;
using BaseRest.Core.Model.Base;
using BaseRest.Core.Model.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;

namespace BaseUI.Services
{
    public class GenderService
    {
        private readonly WebApiClient baseApiClient;
        private IHttpContextAccessor contextAccessor;
        private readonly NavigationManager navigator;

        public GenderService(WebApiClient baseApiClient, IHttpContextAccessor contextAccessor, NavigationManager navigator)
        {
            this.baseApiClient = baseApiClient;
            this.contextAccessor = contextAccessor;
            this.navigator = navigator;
        }

        public async Task<List<GenderDTO>> GetGendersAsync()
        {
            await baseApiClient.ValidateAccessToken(contextAccessor, navigator);
            var result = await baseApiClient.GetAsync<List<GenderDTO>>($"Gender");
            return result;
        }
    }
}

