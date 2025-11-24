using MetricsApp.AppHost.OpenTelemetryCollector;

var builder = DistributedApplication.CreateBuilder(args);

// Conditionally configure the environment based on deployment target
var deployToAzure = builder.Configuration["DEPLOY_TO_AZURE"] == "true";

if (deployToAzure)
{
    // When deploying to Azure (via azd or aspire publish)
    builder.AddAzureContainerAppEnvironment("env");
}
else
{
    // When running locally with Docker Compose (without dashboard)
    builder.AddDockerComposeEnvironment("env").WithDashboard(false);
}

// Add OpenTelemetry Collector
// var otelCollector = builder.AddOpenTelemetryCollector(
//     "otel-collector",
//     "otel-collector-config.yaml"
// );

var apiService = builder
    .AddProject<Projects.AspireDemo_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

// .WithEnvironment(
//     "OTEL_EXPORTER_OTLP_ENDPOINT",
//     otelCollector.GetEndpoint(OpenTelemetryCollectorResource.OtlpGrpcEndpointName)
// );

builder
    .AddProject<Projects.AspireDemo_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

// .WithEnvironment(
//     "OTEL_EXPORTER_OTLP_ENDPOINT",
//     otelCollector.GetEndpoint(OpenTelemetryCollectorResource.OtlpGrpcEndpointName)
// );

builder.Build().Run();
