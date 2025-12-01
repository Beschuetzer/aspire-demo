namespace AspireDemo.AppHost.Monitoring;

public static class TempoResourceBuilderExtensions
{
    /// <summary>
    /// Adds a Tempo container to the application for distributed tracing using a custom Dockerfile with baked-in config.
    /// </summary>
    public static IResourceBuilder<ContainerResource> AddTempo(
        this IDistributedApplicationBuilder builder,
        string name,
        int? httpPort = null
    )
    {
        return builder
            .AddDockerfile(name, "Monitoring/tempo")
            .WithArgs("-config.file=/etc/tempo/local-config.yaml")
            // This WithVolume call is commented out to avoid issues when deploying to Azure Container Apps.
            // Without volumes, data won't persist across container restarts in Azure.
            // If you need persistence in Azure, you'd need to either :
            //      Use Azure Blob Storage or Azure Files for storage backends
            //      Or configure the monitoring tools to use cloud-native storage
            //      (e.g., Loki with Azure Blob Storage)
            // .WithVolume("tempo-data", "/var/tempo")
            .WithHttpEndpoint(port: httpPort, targetPort: 9411, name: "http")
            .WithEndpoint(targetPort: 4317, name: "otlp-grpc", scheme: "http")
            .WithEndpoint(targetPort: 4318, name: "otlp-http", scheme: "http");
    }
}
