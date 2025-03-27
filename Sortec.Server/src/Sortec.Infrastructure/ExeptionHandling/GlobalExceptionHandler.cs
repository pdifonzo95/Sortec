using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Sortec.Domain.DTOs.Exception;
using Sortec.Domain.Entities;
using System.Text.Json;

namespace Sortec.Infrastructure.ExeptionHandling
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            int statusCode;
            string message;

            if (exception is CustomHttpException customHttpException)
            {
                statusCode = customHttpException.StatusCode;
                message = customHttpException.Message;
            }
            else 
            {
                statusCode = exception switch
                {
                    BadHttpRequestException => StatusCodes.Status400BadRequest,
                    UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                    KeyNotFoundException => StatusCodes.Status404NotFound,
                    _ => StatusCodes.Status500InternalServerError
                };

                message = exception.Message;
            }

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/json";

            var errorJson = JsonSerializer.Serialize(new Response<ErrorDetails>()
            {
              Data = new() 
              {
                StatusCode = statusCode,
                Message = message,
              },
               
               Message = "An exception ocurred in the application.",
               Status = false
            });

            await httpContext.Response.WriteAsync(errorJson, cancellationToken: cancellationToken);

            return true;
        }
    }
}