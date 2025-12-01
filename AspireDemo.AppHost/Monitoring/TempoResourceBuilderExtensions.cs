namespace AspireDemo.AppHost.Monitoring;

public static class TempoResourceBuilderExtensions
{
    private const string TempoImage = "grafana/tempo";
    private const string TempoTag = "2.5.0";

    /// <summary>
    /// Adds a Tempo container to the application for distributed tracing.
    /// </summary>
    public static IResourceBuilder<ContainerResource> AddTempo(
        this IDistributedApplicationBuilder builder,
        string name,
        string configFileLocation,
        int? httpPort = null
    )
    {
        return builder
            .AddContainer(name, TempoImage, TempoTag)
            .WithArgs("-config.file=/etc/tempo/local-config.yaml")
            .WithBindMount(configFileLocation, "/etc/tempo/local-config.yaml", isReadOnly: true)
            .WithVolume("tempo-data", "/var/tempo")
            .WithHttpEndpoint(port: httpPort, targetPort: 9411, name: "http")
            .WithEndpoint(targetPort: 4317, name: "otlp-grpc", scheme: "http")
            .WithEndpoint(targetPort: 4318, name: "otlp-http", scheme: "http")
            .WithContainerRuntimeArgs("--user", "0:0");
    }
}
