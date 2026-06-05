using System.Net;
using System.Text.Json;
using ScadaServer.Domain.Exceptions;
using ScadaServer.Application.DTOs;

namespace ScadaServer.WebApi.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var statusCode = (int)HttpStatusCode.InternalServerError;
            var message = "服务器内部错误";
            object? errors = null;

            if (exception is BusinessException bizEx)
            {
                statusCode = bizEx.StatusCode;
                message = bizEx.Message;
                errors = bizEx.Errors;
            }
            else
            {
                _logger.LogError(exception, "系统未处理异常");
                if (_env.IsDevelopment())
                {
                    message = exception.Message;
                    errors = exception.StackTrace;
                }
            }

            context.Response.StatusCode = statusCode;

            var response = ApiResponse.Fail(message, errors);
            
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
        }
    }
}
