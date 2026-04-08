namespace AiWorkbench.Api.Middleware;

public class ApiKeyMiddleware(IConfiguration config) : IMiddleware
{
    private readonly string apiKey = config["ApiKey"] ?? throw new InvalidOperationException("ApiKey is missing in configuration.");

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var path = context.Request.Path.Value?.ToLower();

        if (ShouldBypassApiKey(path))
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("X-Api-Key", out var providedKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("API Key missing.");
            return;
        }

        if (!string.Equals(providedKey, apiKey, StringComparison.Ordinal))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Invalid API Key.");
            return;
        }

        await next(context);
    }

    private static bool ShouldBypassApiKey(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        string[] ignored = { "/api/auth", "/api/chat", "/api/documents", "/api/rag", "/api/speech", "/api/embeddings", "/api/images", "/scalar", "/openapi", "/health" };

        foreach (var prefix in ignored)
        {
            if (path.StartsWith(prefix))
                return true;
        }

        return false;
    }
}
