using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using AuthenticationService = BaseUI.Services.AuthenticationService;
using Newtonsoft.Json;
using System.Security.Claims;
using BaseRest.Core.API.Common;

namespace BaseUI.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        #region Vars
        private readonly IHttpClientFactory httpFactory;
        private readonly AuthenticationService userService;

        [BindProperty(SupportsGet = true)]
        public string jsonStr { get; set; }
        private readonly ILogger<LogoutModel> _logger;
        #endregion


        public LoginModel( IHttpClientFactory httpFactory, AuthenticationService userService, ILogger<LogoutModel> logger)
        {
            this.httpFactory = httpFactory;
            this.userService = userService;
            _logger = logger;
        }

        public async Task logoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = "/")
        {
            returnUrl ??= Url.Content("~/");
            //Limpio tokens y claims antes de ingresar
            await logoutAsync();

            if (string.IsNullOrEmpty(jsonStr))
                return LocalRedirect("/LoginPage");

            JwtAuthResult messageAuthenticationResult = JsonConvert.DeserializeObject<JwtAuthResult>(jsonStr);
            var claims = new List<System.Security.Claims.Claim>();
            if (!string.IsNullOrEmpty(messageAuthenticationResult.AccessToken))
            {
                claims.AddRange(ServiceExtensions.ParseClaimsFromJwt(messageAuthenticationResult.AccessToken));
                claims.Add(new Claim("refreshToken", messageAuthenticationResult.RefreshToken.TokenString));
            }
            var authProperties = new AuthenticationProperties();
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
            return LocalRedirect(returnUrl);
        }
    }
}
