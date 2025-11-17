var builder = DistributedApplication.CreateBuilder(args);

// Add the following line to configure the Azure App Container environment
builder.AddAzureContainerAppEnvironment("env");

var apiService = builder.AddProject<Projects.AspireDemo_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.AspireDemo_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
