using AspireDemo.AppHost.Monitoring;
using MetricsApp.AppHost.OpenTelemetryCollector;

var builder = DistributedApplication.CreateBuilder(args);

// Conditionally configure the environment based on deployment target
var deployToAzure = builder.Configuration["DEPLOY_TO_AZURE"] == "true";

if (deployToAzure)
{
    // When deploying to Azure (via azd or aspire publish)
    builder
        .AddAzureContainerAppEnvironment("env")
        .WithDashboard(!builder.ExecutionContext.IsPublishMode);
}
else
{
    // When running locally with Docker Compose (without dashboard)
    builder
        .AddDockerComposeEnvironment("env")
        .WithDashboard(!builder.ExecutionContext.IsPublishMode);
}

// Add monitoring stack when deploying
// Note: Port mapping is only for local development; Azure Container Apps handles ports automatically
var prometheus = builder.AddPrometheus(
    "prometheus",
    "Monitoring/prometheus.yml",
    port: deployToAzure ? null : 9090
);
var loki = builder.AddLoki(
    "loki",
    "Monitoring/loki-config.yaml",
    port: deployToAzure ? null : 3100
);
var tempo = builder.AddTempo(
    "tempo",
    "Monitoring/tempo-config.yaml",
    httpPort: deployToAzure ? null : 9411
);
var grafana = builder
    .AddGrafana("grafana", "Monitoring/grafana-provisioning", port: deployToAzure ? null : 3000)
    .WaitFor(prometheus)
    .WaitFor(loki)
    .WaitFor(tempo);

// Add OpenTelemetry Collector - receives telemetry from apps and forwards to monitoring stack
var otelCollector = builder
    .AddOpenTelemetryCollector("otel-collector", "otel-collector-config.yaml")
    .WaitFor(prometheus)
    .WaitFor(tempo);

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
