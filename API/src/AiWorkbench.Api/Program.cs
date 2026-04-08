using AiWorkbench.Api.Extensions;
using AiWorkbench.Api.Hubs;
using AiWorkbench.Infrastructure.Extensions;
using AiWorkbench.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AiWorkbenchDbContext>(options => options.UseSqlServer(connection, sqlOptions => sqlOptions.MigrationsAssembly("AiWorkbench.Infrastructure")));

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddInfrastructure();

builder.Services.AddApiHubServices(builder.Configuration);

builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);

builder.Services.AddCorsAndHealthChecks();

builder.Services.AddHealthChecks();

builder.Services.AddRateLimiting();

builder.Services.AddSignalR();

var app = builder.Build();

app.MapOpenApi();

app.MapScalarApiReference();

app.UseErrorHandling();

app.UseHttpsRedirection();

app.UseCorsSupport();

app.UseRateLimitingSupport();

app.UseAuthentication();

app.UseAuthorization();

app.UseApiKey();

app.MapHealthEndpoints();

app.MapControllers();

app.MapHub<SessionHub>("/hubs/session");

app.Run();