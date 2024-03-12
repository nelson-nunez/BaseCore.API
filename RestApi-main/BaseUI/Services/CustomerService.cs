using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BaseRest.Core.API.Common;
using BaseRest.Core.Model.Base;
using BaseRest.Core.Model.DTO;
using Castle.Core.Resource;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;

namespace BaseUI.Services
{
    public class CustomerService
    {
        private readonly WebApiClient baseApiClient;
        private IHttpContextAccessor contextAccessor;
        private readonly NavigationManager navigator;

        public CustomerService(WebApiClient baseApiClient, IHttpContextAccessor contextAccessor, NavigationManager navigator)
        {
            this.baseApiClient = baseApiClient;
            this.contextAccessor = contextAccessor;
            this.navigator = navigator;
        }

        public async Task<CustomerDTO> GetCustomerbyIdAsync(int id)
        {
            var response = await httpClient.GetAsync(resource);


            await baseApiClient.ValidateAccessToken(contextAccessor, navigator);
            var result = await baseApiClient.GetAsync<CustomerDTO>($"Customer/{id}");
            return result;
        }

        public async Task<object> GetPagedCustomersAsync(DataManagerRequest dm)
        {
            var data = new DataResult();
            var request = new PagingSortFilterRequest();
            request.OnlyEnabled = true;

            if (dm.Search != null && dm.Search.Count > 0)
            {
                request.FilterValue = dm.Search.FirstOrDefault().Key.ToLower();
            }
            if (dm.Take != 0)
                request.PageIndex = dm.Skip / dm.Take;

            request.PageSize = dm.Take;

            if (dm.Sorted != null && dm.Sorted.Count > 0)
            {
                var sortDirection = dm.Sorted[0].Direction == "ascending" ? "" : "-";
                request.OrderBy = sortDirection + dm.Sorted[0].Name;
            }

            var queryString = $"/Customer/PagedData?" +
                $"pageIndex={request.PageIndex}" +
                $"&pageSize={request.PageSize}" +
                $"&orderBy={request.OrderBy}" +
                $"&onlyEnabled={request.OnlyEnabled}";

            await baseApiClient.ValidateAccessToken(contextAccessor, navigator);
            var documentList = await baseApiClient.GetAsync<PagedDataResponse<CustomerDTO>>(queryString);           
            data.Result = documentList.Results;
            data.Count = documentList.RowCount;
            return data;
        }

        public async Task<ActionResultDTO> SaveCustomerAsync(CustomerDTO dto)
        {
            await baseApiClient.ValidateAccessToken(contextAccessor, navigator);
            var result = await baseApiClient.PostAsync<ActionResultDTO>("Customer", dto);
            return result;
        }
        
        public async Task<ActionResultDTO> UpdateCustomerAsync(CustomerDTO dto)
        {
            await baseApiClient.ValidateAccessToken(contextAccessor, navigator);
            var result = await baseApiClient.PutAsync<ActionResultDTO>("Customer", dto);
            return result;
        }

        public async Task<ActionResultDTO> DeleteCustomerAsync(int id)
        {
            await baseApiClient.ValidateAccessToken(contextAccessor, navigator);
            var result = await baseApiClient.DeleteAsync<ActionResultDTO>($"Customer/{id}");
            return result;
        }
    }
}

