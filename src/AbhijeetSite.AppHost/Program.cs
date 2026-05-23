var builder = DistributedApplication.CreateBuilder(args);

// Register the API project
var api = builder.AddProject<Projects.AbhijeetSite_Api>("api");

// Register the React frontend (use the 'dev' script for Vite projects)
builder.AddNpmApp("web", "../AbhijeetSite.Web", "dev")
    .WithReference(api)
    .WithEnvironment("VITE_API_URL", api.GetEndpoint("http"))
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints();

builder.Build().Run();
