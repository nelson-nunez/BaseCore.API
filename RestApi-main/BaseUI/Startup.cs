using BaseRest.Core.API.Common;
using BaseUI.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Syncfusion.Blazor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http.Headers;

namespace BaseUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            #region HttpClient

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpContextAccessor(); //Permite obtener el contexto del usuario o la sesion actual

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                //options.Cookie.Name = "AUTH_COOKIE";
                //options.ExpireTimeSpan = TimeSpan.FromMinutes(1);
                options.SlidingExpiration = true;
                options.LoginPath = "/LoginPage";
                options.AccessDeniedPath = "/AccessDenied";
            });

            services.AddHttpClient<WebApiClient>("BaseApiConfig", client =>
            {
                var webApiConfig = AppConfiguration.GetConfigurationSection<WebApiConfig>("BaseApiConfig");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/x-json"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/javascript"));
                client.BaseAddress = new Uri(webApiConfig.BaseAddress);
                client.Timeout = TimeSpan.FromMinutes(webApiConfig.Timeout);
            });

            #endregion

            #region Base Razor Services

            services.AddRazorPages();

            services.AddServerSideBlazor();

            services.AddSyncfusionBlazor();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>()
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("es-ES")
                };
                // Set the default culture 
                var culture = new CultureInfo("es-ES");
                culture.NumberFormat.NumberDecimalSeparator = ".";
                culture.NumberFormat.NumberGroupSeparator = ",";
                options.DefaultRequestCulture = new RequestCulture(culture);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders = new List<IRequestCultureProvider>() {
                 new QueryStringRequestCultureProvider() // Here, You can also use other localization provider 
                };
            });

            #endregion

            #region Syncfusion 

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjQ5MTY3N0AzMTM5MmUzMjJlMzBoQzFJT1BOTnNSbkpkT2lZd2w1RFBJeGxrYkdvUFVBMFp1bzhpQkpYaWU0PQ==");

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddSingleton(typeof(ISyncfusionStringLocalizer), typeof(SyncfusionLocalizer));

            #endregion

         
            #region Servicios

            services.AddLocalization();
            services.AddControllers();
            services.AddInfraestructureServices();

            #endregion
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRequestLocalization(new RequestLocalizationOptions().AddSupportedCultures(new[] { "en-US", "en-US" }).AddSupportedUICultures(new[] { "en-US", "en-US" }));
            app.UseRouting();           
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //Esto evita que se cierre la conexion y aparezca el cartel de: "Atempting to reconect to server 1 of 8"
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
