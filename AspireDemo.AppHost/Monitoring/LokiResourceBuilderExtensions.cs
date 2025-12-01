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
            .WithVolume("loki-storage", "/loki")
            .WithVolume("loki-wal", "/wal")
            .WithHttpEndpoint(port: port, targetPort: 3100, name: "http");
    }
}
