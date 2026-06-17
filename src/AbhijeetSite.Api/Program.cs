using AbhijeetSite.Api.Features.Home;
using AbhijeetSite.Api.Features.Profile;
using AbhijeetSite.Api.Infrastructure.Persistence;
using AbhijeetSite.Api.SharedKernel.Time;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults (OpenTelemetry, metrics, service discovery, etc.)
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddCors();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IApplicationClock, SystemApplicationClock>();
builder.Services.AddPersistence(builder.Configuration);

var app = builder.Build();

app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod());

// Map default endpoints (health check, etc.)
app.MapDefaultEndpoints();

// Ensure health checks are available in production for Azure Container Apps probes
if (!app.Environment.IsDevelopment())
{
    app.MapHealthChecks("/health");
    app.MapHealthChecks("/alive", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        Predicate = r => r.Tags.Contains("live")
    });
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Skip HTTPS redirection when running in a container (Azure Container Apps handles SSL termination at Ingress)
if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
{
    app.UseHttpsRedirection();
}

// Register the endpoints from the Home feature slice
app.MapHomeEndpoints();
app.MapProfileEndpoints();

app.Run();

// Expose the Program class for integration testing
public partial class Program { }
