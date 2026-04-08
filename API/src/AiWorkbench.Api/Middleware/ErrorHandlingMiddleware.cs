using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace AiWorkbench.Api.Middleware;

public sealed class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IHostEnvironment env)
{
    private readonly RequestDelegate next = next ?? throw new ArgumentNullException(nameof(next));
    private readonly ILogger<ErrorHandlingMiddleware> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IHostEnvironment env = env ?? throw new ArgumentNullException(nameof(env));

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex).ConfigureAwait(false);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier ?? Guid.NewGuid().ToString();
        var (statusCode, title, detail, errors) = MapExceptionToProblem(exception);

        logger.LogError(exception, "Unhandled exception occurred. TraceId: {TraceId}, Path: {Path}", traceId, context.Request.Path);

        var problem = new ProblemDetails
        {
            Type = $"https://httpstatuses.io/{(int)statusCode}",
            Title = title,
            Status = (int)statusCode,
            Detail = env.IsDevelopment() ? detail : null,
            Instance = context.Request.Path
        };

        if (errors is not null && errors.Any())
        {
            foreach (var kv in errors)
            {
                problem.Extensions[kv.Key] = kv.Value;
            }
        }

        problem.Extensions["traceId"] = traceId;

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        await JsonSerializer.SerializeAsync(context.Response.Body, problem, options).ConfigureAwait(false);
    }

    private static (HttpStatusCode StatusCode, string Title, string Detail, Dictionary<string, object>? Errors) MapExceptionToProblem(Exception ex)
    {
        switch (ex)
        {
            case DbUpdateConcurrencyException _:
                return (HttpStatusCode.Conflict, "Concurrency conflict", ex.Message, null);

            case DbUpdateException _:
                return (HttpStatusCode.InternalServerError, "Database update error", ex.Message, null);

            case ValidationException vex:
                return (HttpStatusCode.BadRequest, "Validation error", vex.Message, new Dictionary<string, object> { ["validationErrors"] = new[] { vex.Message } });

            case FluentValidation.ValidationException fex:
                var errors = fex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => (object)g.Select(e => e.ErrorMessage).ToArray());
                return (HttpStatusCode.BadRequest, "Validation failed", "One or more validation errors occurred.", errors);

            case UnauthorizedAccessException _:
                return (HttpStatusCode.Unauthorized, "Unauthorized", ex.Message, null);

            case KeyNotFoundException _:
                return (HttpStatusCode.NotFound, "Not found", ex.Message, null);

            default:
                return (HttpStatusCode.InternalServerError, "An unexpected error occurred", ex.Message, null);
        }
    }
}