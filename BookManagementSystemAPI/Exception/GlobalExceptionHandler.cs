﻿using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BookManagementSystemAPI.Exception
{
    public class GlobalExceptionHandler
    {
        private ILogger<GlobalExceptionHandler> _logger;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }
        public async Task HandleExceptionAsync(HttpContext context)
        {
            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
            if (exceptionHandlerFeature == null) return;

            var exception = exceptionHandlerFeature.Error;

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = GetStatusCode(exception);

            var response = new FormattedResponse(exception.Message, false, context.Response.StatusCode);

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private int GetStatusCode(System.Exception exception)
        {
            if (exception is ArgumentException)
            {
                return 400;
            }

            else if (exception is DbUpdateException)
            {
                return 500;
            }

            else if (exception is InvalidOperationException)
            {
                return 500;
            }

            else if (exception is KeyNotFoundException)
            {
                return 400;
            }

            else if (exception is NotFoundException)
            {
                return 404;
            }

            else
            {
                _logger.LogError($"error: {exception.Message}");

                return 500;
            }
        }
    }
}
