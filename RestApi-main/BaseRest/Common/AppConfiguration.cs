using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BaseRest.Core.API.Common
{
    public static class AppConfiguration
    {
        public static IConfiguration Configuration;

        static AppConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var envDiscriminator = !string.IsNullOrWhiteSpace(environment) ? $".{environment}" : string.Empty;
            var currentDirectory = Path.GetDirectoryName(Assembly.GetAssembly(typeof(AppConfiguration)).Locati‌​on);

            var builder = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile($"appsettings{envDiscriminator}.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
        }

        public static string GetConnectionString(string name)
        {
            return Configuration.GetConnectionString(name);
        }

        public static string GetConfiguration(string key)
        {
            return Configuration[key];
        }

        public static T GetConfigurationSection<T>(string sectionName)
        {
            var section = Configuration.GetSection(sectionName);
            var sectionT = section.Get<T>();

            return sectionT;
        }
    }
}
