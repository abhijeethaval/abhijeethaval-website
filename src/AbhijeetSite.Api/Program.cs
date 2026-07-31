using AbhijeetSite.Api.Features.Articles;
using AbhijeetSite.Api.Features.Articles.Admin;
using AbhijeetSite.Api.Features.Home;
using AbhijeetSite.Api.Features.Identity;
using AbhijeetSite.Api.Features.Profile;
using AbhijeetSite.Api.Infrastructure.Observability;
using AbhijeetSite.Api.Infrastructure.Persistence;
using AbhijeetSite.Api.SharedKernel.Time;
using Microsoft.AspNetCore.HttpOverrides;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

const string ApiServiceName = "abhijeetsite-api";

// Add service defaults (OpenTelemetry, metrics, service discovery, etc.)
builder.AddServiceDefaults(
    ApiServiceName,
    ArticlesTelemetry.SourceName,
    IdentityTelemetry.SourceName);

// Add services to the container.
builder.Services.AddCors();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IApplicationClock, SystemApplicationClock>();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddIdentityAuthentication(builder.Configuration, builder.Environment);

var app = builder.Build();

await app.InitializeDatabaseAsync();

app.UseExceptionHandler();
app.UseForwardedHeaders();
app.UseIdentityPublicOrigin();

app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod());

// Map default endpoints (health check, etc.)
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Skip HTTPS redirection in local development and containers. Vite and ACA own the browser-facing edge.
if (!app.Environment.IsDevelopment() && Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

// Register the endpoints from the Home feature slice
app.MapHomeEndpoints();
app.MapProfileEndpoints();
app.MapArticleEndpoints();
app.MapAdminArticleEndpoints();
app.MapIdentityEndpoints();

app.Run();

// Expose the Program class for integration testing
public partial class Program { }
