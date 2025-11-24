using MetricsApp.AppHost.OpenTelemetryCollector;

var builder = DistributedApplication.CreateBuilder(args);

// Add the following line to configure the Azure App Container environment
//builder.AddAzureContainerAppEnvironment("env");
builder.AddDockerComposeEnvironment("env");

// Add OpenTelemetry Collector
var otelCollector = builder.AddOpenTelemetryCollector(
    "otel-collector",
    "otel-collector-config.yaml"
);

var apiService = builder
    .AddProject<Projects.AspireDemo_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithEnvironment(
        "OTEL_EXPORTER_OTLP_ENDPOINT",
        otelCollector.GetEndpoint(OpenTelemetryCollectorResource.OtlpGrpcEndpointName)
    );

builder
    .AddProject<Projects.AspireDemo_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithEnvironment(
        "OTEL_EXPORTER_OTLP_ENDPOINT",
        otelCollector.GetEndpoint(OpenTelemetryCollectorResource.OtlpGrpcEndpointName)
    );

builder.Build().Run();
