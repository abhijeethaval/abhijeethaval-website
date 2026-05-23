using AbhijeetSite.Api.Features.Home;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults (OpenTelemetry, metrics, service discovery, etc.)
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddCors();
builder.Services.AddOpenApi();

var app = builder.Build();

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

app.UseHttpsRedirection();

// Register the endpoints from the Home feature slice
app.MapHomeEndpoints();

app.Run();

// Expose the Program class for integration testing
public partial class Program { }
