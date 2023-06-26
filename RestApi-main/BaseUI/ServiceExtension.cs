using BaseUI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BaseUI
{
    public static class ServiceExtension
    {
        public static void AddInfraestructureServices(this IServiceCollection services)
        {
            services.AddScoped<Syncfusion.Blazor.Spinner.SfSpinner>();

            services.AddScoped<AuthenticationService>();
          
            services.AddScoped<CustomerService>();
          
            services.AddScoped<GenderService>();

        }
    }
}