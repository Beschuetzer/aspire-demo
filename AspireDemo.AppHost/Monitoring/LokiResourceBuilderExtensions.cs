namespace AspireDemo.AppHost.Monitoring;

public static class LokiResourceBuilderExtensions
{
    /// <summary>
    /// Adds a Loki container to the application for log aggregation using a custom Dockerfile with baked-in config.
    /// </summary>
    public static IResourceBuilder<ContainerResource> AddLoki(
        this IDistributedApplicationBuilder builder,
        string name,
        int? port = null
    )
    {
        return builder
            .AddDockerfile(name, "Monitoring/loki")
            .WithArgs("-config.file=/etc/loki/local-config.yaml")
            // These WithVolume calls are commented out to avoid issues when deploying to Azure Container Apps.
            // Without volumes, data won't persist across container restarts in Azure.
            // If you need persistence in Azure, you'd need to either :
            //      Use Azure Blob Storage or Azure Files for storage backends
            //      Or configure the monitoring tools to use cloud-native storage
            //      (e.g., Loki with Azure Blob Storage)
            // .WithVolume("loki-storage", "/loki")
            // .WithVolume("loki-wal", "/wal")
            .WithHttpEndpoint(port: port, targetPort: 3100, name: "http");
    }
}
