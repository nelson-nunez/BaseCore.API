using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseRest.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region SERILOG
            //var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            //var columnOptionsSection = configuration.GetSection("Serilog:ColumnOptions");
            //var sinkOptionsSection = configuration.GetSection("Serilog:SinkOptionsSection");
            //Log.Logger = new LoggerConfiguration()
            //    .WriteTo.MSSqlServer(
            //        connectionString: "Server=DESKTOP-BF4I4OK\\MSSQLSERVER2019;Database=DatabaseCore;User ID=sa;Password=123456;MultipleActiveResultSets=true; TrustServerCertificate=True",
            //        sinkOptions: new MSSqlServerSinkOptions
            //        {
            //            TableName = "LogEvents",
            //            SchemaName = "dbo",
            //            AutoCreateSqlTable = true
            //        },
            //        sinkOptionsSection: sinkOptionsSection,
            //        appConfiguration: configuration,
            //        columnOptionsSection: columnOptionsSection)
            //    .CreateLogger();
            //Log.Information("{UserId}", Environment.UserName);
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
                })
                //.UseSerilog();
                .UseSerilog((HostBuilderContext context, LoggerConfiguration loggerConfiguration) =>
                {
                    loggerConfiguration.ReadFrom.Configuration(context.Configuration)
                                       .Enrich.FromLogContext();
                });
    }
}
