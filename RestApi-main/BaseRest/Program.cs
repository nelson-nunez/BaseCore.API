using BaseRest.Core.API.LogsConfiguration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BaseRest.Core
{
    public class Program
    {
        public IConfiguration Configuration { get; }

        public static void Main(string[] args)
        {
            #region SERILOG
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            var connectionStringSection = configuration.GetSection("ConnectionStrings:LogsConnection");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Filter.ByExcluding((le) => le.Level == LogEventLevel.Information) 
                .Filter.ByExcluding((le) => le.Level == LogEventLevel.Debug) 
                .Enrich.WithProcessId()
                .Enrich.WithProcessName()
                .Enrich.FromLogContext()
                .WriteTo.MSSqlServer(
                    connectionString: connectionStringSection.Value,
                    appConfiguration: configuration,
                    sinkOptions: SerilogConfiguration.GetsSinkOptions(),
                    columnOptions: SerilogConfiguration.GetColumnOptions())
                .CreateLogger();
            #endregion

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Debug);
                    logging.SetMinimumLevel(LogLevel.Information);
                })
                .UseSerilog();
    }
}
