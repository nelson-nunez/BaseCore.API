using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BaseRest.Core.API
{
    //Esto no se está usando todavíaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
    public class ExceptionHandlingMiddleware
    {
        public RequestDelegate requestDelegate;
        private readonly ILogger<ExceptionHandlingMiddleware> logger;
        public ExceptionHandlingMiddleware
        (RequestDelegate requestDelegate, ILogger<ExceptionHandlingMiddleware> logger)
        {
            this.requestDelegate = requestDelegate;
            this.logger = logger;
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
            logger.LogError(ex.ToString());
            var errorMessageObject =
                new { Message = ex.Message, Code = "system_error" };

            var errorMessage = JsonConvert.SerializeObject(errorMessageObject);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(errorMessage);
        }
    }
}