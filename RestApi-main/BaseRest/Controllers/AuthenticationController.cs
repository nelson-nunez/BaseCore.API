using BaseRest.Core.API.Common;
using BaseRest.Core.Business;
using BaseRest.Core.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BaseRest.Core.Controllers
{
    [AllowAnonymous]
    public class AuthenticationController : BaseController
    {
        private readonly AuthenticationBusiness authenticationBusiness;
        private readonly UnitOfWork unitOfWork;
        public IConfiguration Configuration { get; }

        //TODO: solo para pureba
        private readonly ILogger<AuthenticationController> logger;


        public AuthenticationController(AuthenticationBusiness authenticationBusiness, UnitOfWork unitOfWork, IConfiguration configuration, ILogger<AuthenticationController> logger, IJwtAuthManager jwtAuthManager) : base(jwtAuthManager)
        {
            this.authenticationBusiness = authenticationBusiness;
            this.unitOfWork = unitOfWork;
            Configuration = configuration;
            this.logger = logger;
        }

        //Se podria agregar url encoded para mayor seguridad, tambien un objeto request signin
        [HttpPost("Authenticate")]
        public async Task<ActionResult> Authenticate([FromBody] SigninRequest request)
        {

            if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
                throw new Exception("Usuario y Contraseña requeridos.");

            await authenticationBusiness.Authenticate(request.UserName, request.Password);
      
            #region Token
            var claimList = new[] 
            { 
                new Claim("UserId", request.UserName),
                new Claim(ClaimTypes.Name, request.UserName),
            };

            var jwtResult = jwtAuthManager.GenerateTokens(request.UserName, claimList, DateTime.Now, 200);
            #endregion

            return Ok(jwtResult);           
        }
    }
}
