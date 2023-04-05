using BaseRest.Core.Business;
using BaseRest.Core.DataAccess;
using BaseRest.Core.DataAccess.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseRest.Core.API
{

    public static class ServiceExtension
    {
        public static void AddInfraestructureServices(this IServiceCollection services)
        {
            services.AddSingleton((LoggerConfiguration)
                new LoggerConfiguration().ReadFrom.Configuration(AppConfiguration.Configuration, "SerilogErrorLog").Enrich.FromLogContext());
        }

        public static void AddDataAccessServices(this IServiceCollection services)
        {
            services.AddScoped<UnitOfWork, UnitOfWork>();

            services.AddScoped<CustomerRepository, CustomerRepository>();

            services.AddScoped<AppUserRepository, AppUserRepository>();
        }

        public static void AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<CustomerBusiness, CustomerBusiness>();

            services.AddScoped<AuthenticationBusiness, AuthenticationBusiness>();
        }
    }
}

