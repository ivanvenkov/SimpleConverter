using jConverter.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Xml;

namespace JConverter.Midllewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleware> logger;
        private readonly IHostEnvironment env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            this.next = next;
            this.logger = logger;
            this.env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await this.next(context);
            }
            catch (FileExistsException ex)
            {
                this.logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 400;

                var resposne = new ProblemDetails
                {
                    Status = 400,
                    Detail = this.env.IsDevelopment() ? ex.StackTrace?.ToString() : null,
                    Title = ex.Message
                };

                await RetrunResponse(context, resposne);
            }
            catch (XmlException ex)
            {
                this.logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 400;

                var resposne = new ProblemDetails
                {
                    Status = 400,
                    Detail = this.env.IsDevelopment() ? ex.StackTrace?.ToString() : null,
                    Title = "Please upload a valid xml file with an .xml extension."
                };

                await RetrunResponse(context, resposne);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500;

                var resposne = new ProblemDetails
                {
                    Status = 500,
                    Detail = this.env.IsDevelopment() ? ex.StackTrace?.ToString() : null,
                    Title = ex.Message
                };

                await RetrunResponse(context, resposne);
            }
        }

        private static async Task RetrunResponse(HttpContext context, ProblemDetails resposne)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(resposne, options);
            await context.Response.WriteAsync(json);
        }
    }
}