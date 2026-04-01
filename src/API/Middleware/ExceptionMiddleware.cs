using System;
using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace API.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch(ValidationException ex)
        {
            await HandleValidationException(context, ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleValidationException(HttpContext context, ValidationException ex)
    {
        var validationErrros = new Dictionary<string, string[]>();

        if (ex.Errors is not null)
        {
            foreach (var error in ex.Errors)
            {
                if (validationErrros.TryGetValue(error.PropertyName, out var existingErrors))
                    validationErrros[error.PropertyName] = [.. existingErrors, error.ErrorMessage];
                else
                    validationErrros[error.PropertyName] = [error.ErrorMessage];
            }
        }

        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        var validationProblemDetails = new ValidationProblemDetails(validationErrros)
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "ValidationFailure",
            Title = "Validation error",
            Detail = "One or more validation error has occured"
        };

        await context.Response.WriteAsJsonAsync(validationProblemDetails);
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (statusCode, message) = ex switch
        {
            KeyNotFoundException => (HttpStatusCode.NotFound, ex.Message),
            UnauthorizedAccessException => (HttpStatusCode.Forbidden, ex.Message),
            InvalidOperationException => (HttpStatusCode.BadRequest, ex.Message),
            ArgumentException => (HttpStatusCode.BadGateway, ex.Message),
            _ => (HttpStatusCode.InternalServerError, env.IsDevelopment() ? ex.Message : "An unexpected error occurred."),
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var body = JsonSerializer.Serialize(new
        {
            error = message,
            statusCode = (int)statusCode
        });

        return context.Response.WriteAsync(body);
    }
}

