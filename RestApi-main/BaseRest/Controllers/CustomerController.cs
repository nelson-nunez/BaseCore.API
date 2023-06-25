using AutoMapper;
using BaseRest.Core.Model.Base;
using BaseRest.Core.Business;
using BaseRest.Core.Model;
using BaseRest.Core.Model.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseRest.Core.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class CustomerController : Controller
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IMapper mapper;
        private readonly CustomerBusiness customerBusiness;

        public CustomerController(IMapper mapper, IHttpContextAccessor contextAccessor, CustomerBusiness customerBusiness)
        {
            this.mapper = mapper;
            this.contextAccessor = contextAccessor;
            this.customerBusiness = customerBusiness;
        }

        [HttpGet()]
        public async Task<ActionResult<CustomerDTO>> GetAllResellers()
        {
            var list = await customerBusiness.GetAsync();
            var dto = mapper.Map<IList<CustomerDTO>>(list);
            return Ok(dto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDTO>> GetCustomerbyId(int id)
        {
            var item = await customerBusiness.FindAsync(id);
            var dto = mapper.Map<CustomerDTO>(item);
            return Ok(dto);
        }

        [HttpGet("PagedData")]
        public async Task<ActionResult<PagedDataResponse<CustomerDTO>>> GetAllResellers([FromQuery] PagingSortFilterRequest request)
        {
            //Fix orderBy properties out entity
            var orderDirection = "";
            if (!string.IsNullOrEmpty(request.OrderBy) && request.OrderBy[0] == '-')
            {
                orderDirection = "-";
                request.OrderBy = request.OrderBy.Substring(1);
            }
            switch (request.OrderBy)
            {
                case "Name":
                    request.OrderBy = $"{orderDirection}Person.Name";
                    break;
                default:
                    request.OrderBy = "-Id";
                    break;
            }

            var result = await customerBusiness.GetPagedResultAsync(request);
            var response = new PagedDataResponse<CustomerDTO> { Results = new List<CustomerDTO>() };
            response.PageCount = result.PageCount;
            response.PageIndex = result.PageIndex;
            response.PageSize = result.PageSize;
            response.RowCount = result.RowCount;
            response.Results = mapper.Map<List<CustomerDTO>>(result.Results);
            return Ok(response);
        }

        [HttpPost()]
        public async Task<ActionResult<ActionResultDTO>> Add([FromBody] CustomerDTO dto)
        {
            var entity = mapper.Map<Customer>(dto);
            var result = await customerBusiness.CustomerSaveAsync(entity);
            var response = new ActionResultDTO()
            {
                Code = result.ToString(),
                Message = "El cliente se registró correctamente"
            };
            return Ok(response);
        }

        [HttpPut()]
        public async Task<ActionResult<ActionResultDTO>> Update([FromBody] CustomerDTO dto)
        {
            var entityFromDb = await customerBusiness.FindAsync(dto.Id);
            if (entityFromDb == null)
                throw new Exception("No se encontró el registro");

            entityFromDb.Map(dto);
            var result = await customerBusiness.CustomerSaveAsync(entityFromDb);
            var response = new ActionResultDTO()
            {
                Code = result.ToString(),
                Message = "El cliente se actualizó correctamente"
            };
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ActionResultDTO>> Delete([FromRoute] int id)
        {
            var result = await customerBusiness.DeleteAsync(id);
            var response = new ActionResultDTO()
            {
                Code = result.ToString(),
                Message = "El cliente se eliminó correctamente"
            };
            return Ok(response);
        }
    }

    public static class CustomerExtensions
    {
        public static void Map(this Customer entity, CustomerDTO dto)
        {
            entity.Name = dto.Name;
            entity.BirthDate = dto.BirthDate;
            entity.CUIL = dto.CUIL;
            entity.GenderId = dto.GenderId > 0 ? dto.GenderId : entity.GenderId;
            entity.Phone = dto.Phone;
        }
    }
}
