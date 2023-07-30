using jConverter.Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace jConverter.Application.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        private readonly ILogger<ValidationFilter> logger;

        public ValidationFilter(ILogger<ValidationFilter> logger) => (this.logger) = logger;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var response = CreateResponseModel(context);

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.StatusCode = 400;
                var json = JsonSerializer.Serialize(response, options);
                await context.HttpContext.Response.WriteAsync(json);
                return;
            }

            await next();
        }

        private ErrorModel CreateResponseModel(ActionExecutingContext context)
        {
            ErrorModel response = new();

            response.ErrorMsg = string.Join(
                                             Environment.NewLine,
                                             context.ModelState.Values.Where(e => e.Errors.Count > 0)
                         .SelectMany(E => E.Errors)
                         .Select(E => E.ErrorMessage)
                         .ToList());

            response.Time = DateTime.Now.ToString();
            LogErrors(response);
          
            return response;
        }

        private bool LogErrors(ErrorModel response)
        {
            this.logger.LogError($"Validation error {DateTime.Now} {response.ErrorMsg}");
            return true;
        }
    }
}
