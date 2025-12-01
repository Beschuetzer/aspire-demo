namespace AspireDemo.AppHost.Monitoring;

public static class LokiResourceBuilderExtensions
{
    private const string LokiImage = "grafana/loki";
    private const string LokiTag = "2.8.2";

    /// <summary>
    /// Adds a Loki container to the application for log aggregation.
    /// </summary>
    public static IResourceBuilder<ContainerResource> AddLoki(
        this IDistributedApplicationBuilder builder,
        string name,
        string configFileLocation,
        int? port = null
    )
    {
        return builder
            .AddContainer(name, LokiImage, LokiTag)
            .WithArgs("-config.file=/etc/loki/local-config.yaml")
            .WithBindMount(configFileLocation, "/etc/loki/local-config.yaml", isReadOnly: true)
            .WithVolume("loki-storage", "/loki")
            .WithVolume("loki-wal", "/wal")
            .WithHttpEndpoint(port: port, targetPort: 3100, name: "http")
            .WithContainerRuntimeArgs("--user", "0:0");
    }
}
