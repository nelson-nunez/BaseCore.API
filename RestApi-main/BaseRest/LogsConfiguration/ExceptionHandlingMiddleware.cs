using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Context;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace BaseRest.Core.API.LogsConfiguration
{
    public class ExceptionHandlingMiddleware
    {
        public RequestDelegate requestDelegate;
        private readonly ILogger<ExceptionHandlingMiddleware> logger;
        private readonly IHttpContextAccessor contextAccessor;

        public ExceptionHandlingMiddleware(RequestDelegate requestDelegate, ILogger<ExceptionHandlingMiddleware> logger, IHttpContextAccessor contextAccessor)
        {
            this.requestDelegate = requestDelegate;
            this.logger = logger;
            this.contextAccessor = contextAccessor;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await requestDelegate(context);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private Task HandleException(HttpContext context, Exception ex)
        {
            var loggeduser = (contextAccessor?.HttpContext?.User?.Identity?.Name != null) ? contextAccessor.HttpContext.User.Identity.Name: "notlogged";
            //Error enriquecido que se guarda por Serilog
            using (LogContext.PushProperty("UserId", Environment.UserName))
            using (LogContext.PushProperty("CreatedBy", loggeduser))
            using (LogContext.PushProperty("ThreadId", Environment.OSVersion))
            //using (LogContext.PushProperty("StackTrace", ex.StackTrace))//Limitar la extensión maxima o ampliar la columna
            {
                logger.LogError(ex.ToString()); 
            }
            //Error visible en sistema
            var errorMessageObject = new { 
                                            Message = ex.Message, 
                                            Code = "system_error",
                                            Stacktrace= ex.StackTrace,
                                            Data= ex.Data,
                                            InnerException= ex.InnerException,
                                            Source = ex.Source
                                         };

            var errorMessage = JsonConvert.SerializeObject(ex);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(errorMessage);
        }
    }
}