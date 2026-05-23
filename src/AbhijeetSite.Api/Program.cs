using AbhijeetSite.Api.Features.Home;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults (OpenTelemetry, metrics, service discovery, etc.)
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Register the endpoints from the Home feature slice
app.MapHomeEndpoints();

app.Run();

// Expose the Program class for integration testing
public partial class Program { }
