using AiWorkbench.Api.Hubs.Services;
using AiWorkbench.Api.Middleware;
using AiWorkbench.Api.Services;
using AiWorkbench.Application.Configuration;
using AiWorkbench.Application.Interfaces;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

namespace AiWorkbench.Api.Extensions
{
    public static class Extensions
    {
        public static IServiceCollection AddCorsAndHealthChecks(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("Default", policy =>
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod());
            });

            return services;
        }
        public static IServiceCollection AddApiHubServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddSingleton<IUserConnectionService, UserConnectionService>();

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

            services.Configure<ExternalServicesSettings>(configuration.GetSection("ExternalServices"));

            services.Configure<AzureOpenAISettings>(configuration.GetSection("ExternalServices:AzureOpenAI"));
            services.Configure<BlobStorageSettings>(configuration.GetSection("ExternalServices:AzureBlobStorage"));
            services.Configure<RagSettings>(configuration.GetSection("ExternalServices:RagService"));
            services.Configure<AzureOpenAISettings>(configuration.GetSection("ExternalServices:AzureOpenAI"));

            services.AddScoped<ApiKeyMiddleware>();

            return services;
        }


        public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";
            })
            .AddJwtBearer("Bearer", options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,

                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)
                    )
                };
            });

            services.AddAuthorization();
            return services;
        }

        public static IServiceCollection AddRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("global", limiterOptions =>
                {
                    limiterOptions.PermitLimit = 100;
                    limiterOptions.Window = TimeSpan.FromMinutes(1);
                    limiterOptions.QueueLimit = 0;
                    limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });
            });

            return services;
        }

        public static IApplicationBuilder UseCorsSupport(this IApplicationBuilder app)
        {
            app.UseCors("Default");
            return app;
        }

        public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks("/health");
            return endpoints;
        }

        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ErrorHandlingMiddleware>();
        }

        public static IApplicationBuilder UseApiKey(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ApiKeyMiddleware>();
        }

        public static IApplicationBuilder UseRateLimitingSupport(this IApplicationBuilder app)
        {
            app.UseRateLimiter();
            return app;
        }
    }
}