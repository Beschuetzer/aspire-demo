using AspireDemo.AppHost.Monitoring;
using MetricsApp.AppHost.OpenTelemetryCollector;

var builder = DistributedApplication.CreateBuilder(args);

// Conditionally configure the environment based on deployment target
var deployToAzure = builder.Configuration["DEPLOY_TO_AZURE"] == "true";

// Determine if we're in publish mode (deploying) vs run mode (local development)
var isPublishMode = builder.ExecutionContext.IsPublishMode;

if (deployToAzure)
{
    // When deploying to Azure (via azd or aspire publish)
    builder.AddAzureContainerAppEnvironment("env").WithDashboard(!isPublishMode);
}
else
{
    // Run mode - use Aspire dashboard for local development
    builder.AddDockerComposeEnvironment("env").WithDashboard(!isPublishMode);
}

// Define the API service first
var apiService = builder
    .AddProject<Projects.AspireDemo_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

var webfrontend = builder
    .AddProject<Projects.AspireDemo_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

// Only add monitoring stack (Grafana + OTEL Collector) when publishing
// In run mode, use the built-in Aspire dashboard instead
if (isPublishMode)
{
    // Add monitoring stack - uses custom Dockerfiles with baked-in configs (no bind mounts)
    var prometheus = builder.AddPrometheus("prometheus", port: deployToAzure ? null : 9090);
    var loki = builder.AddLoki("loki", port: deployToAzure ? null : 3100);
    var tempo = builder.AddTempo("tempo", httpPort: deployToAzure ? null : 9411);
    var grafana = builder
        .AddGrafana("grafana", port: deployToAzure ? null : 3000)
        .WaitFor(prometheus)
        .WaitFor(loki)
        .WaitFor(tempo);

    // Add OpenTelemetry Collector - receives telemetry from apps and forwards to monitoring stack
    var otelCollector = builder
        .AddOpenTelemetryCollector("otel-collector")
        .WaitFor(prometheus)
        .WaitFor(tempo);

    // Configure apps to send telemetry to OTEL collector
    apiService.WithEnvironment(
        "OTEL_EXPORTER_OTLP_ENDPOINT",
        otelCollector.GetEndpoint(OpenTelemetryCollectorResource.OtlpGrpcEndpointName)
    );

    webfrontend.WithEnvironment(
        "OTEL_EXPORTER_OTLP_ENDPOINT",
        otelCollector.GetEndpoint(OpenTelemetryCollectorResource.OtlpGrpcEndpointName)
    );
}

builder.Build().Run();
