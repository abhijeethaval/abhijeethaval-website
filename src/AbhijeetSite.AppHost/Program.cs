var builder = DistributedApplication.CreateBuilder(args);

const string DatabaseServerName = "postgres";
const string ApplicationDatabaseName = "abhijeetsite-db";

var postgres = builder.AddPostgres(DatabaseServerName);
var applicationDatabase = postgres.AddDatabase(ApplicationDatabaseName);

// Register the API project
var api = builder.AddProject<Projects.AbhijeetSite_Api>("api")
    .WithReference(applicationDatabase)
    .WaitFor(applicationDatabase);

// Register the React frontend (use the 'dev' script for Vite projects)
builder.AddNpmApp("web", "../AbhijeetSite.Web", "dev")
    .WithReference(api)
    .WithEnvironment("VITE_API_URL", api.GetEndpoint("http"))
    .WithHttpEndpoint(port: 5173, env: "PORT")
    .WithExternalHttpEndpoints();

builder.Build().Run();
