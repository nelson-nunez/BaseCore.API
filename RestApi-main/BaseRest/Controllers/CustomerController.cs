using AutoMapper;
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
    [Route("api/[controller]")]
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

        //[HttpGet()]
        //public async Task<ActionResult<CustomerDTO>> GetAllResellers([FromQuery] Request request)
        //{
        //    CustomerDTO customer = new CustomerDTO
        //    {
        //        Id = 1,
        //        Name = "Juan Perez",
        //        BirthDate = new DateTime(1990, 1, 1),
        //        CUIL = "12345678910",
        //        GenderId = 1,
        //        Phone = "364412345678",
        //        Created = DateTime.Now,
        //    };
        //    return Ok(customer);
        //}

        [HttpGet()]
        public async Task<ActionResult<CustomerDTO>> GetAllResellers()
        {
            var list = await customerBusiness.GetAsync();
            var dto = mapper.Map<IList<CustomerDTO>>(list);
            return Ok(dto);
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
