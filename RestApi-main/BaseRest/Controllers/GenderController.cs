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
    public class GenderController : Controller
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IMapper mapper;
        private readonly GenderBusiness genderBusiness;

        public GenderController(IMapper mapper, IHttpContextAccessor contextAccessor, GenderBusiness genderBusiness)
        {
            this.mapper = mapper;
            this.contextAccessor = contextAccessor;
            this.genderBusiness = genderBusiness;
        }

        [HttpGet()]
        public async Task<ActionResult<GenderDTO>> GetAllGenders()
        {
            var list = await genderBusiness.GetAsync();
            var dto = mapper.Map<IList<GenderDTO>>(list);
            return Ok(dto);
        }
    }
}
