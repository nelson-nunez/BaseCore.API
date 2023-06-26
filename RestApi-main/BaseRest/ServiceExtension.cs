using BaseRest.Core.API.Common;
using BaseRest.Core.Business;
using BaseRest.Core.DataAccess;
using BaseRest.Core.DataAccess.Repository;
using Castle.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
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

        }

        public static void AddDataAccessServices(this IServiceCollection services)
        {
            services.AddScoped<UnitOfWork, UnitOfWork>();

            services.AddScoped<CustomerRepository, CustomerRepository>();

            services.AddScoped<GenderRepository, GenderRepository>();

            services.AddScoped<AppUserRepository, AppUserRepository>();
        }

        public static void AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<CustomerBusiness, CustomerBusiness>();
            services.AddScoped<GenderBusiness, GenderBusiness>();

            services.AddScoped<AuthenticationBusiness, AuthenticationBusiness>();
        }
    }
}

