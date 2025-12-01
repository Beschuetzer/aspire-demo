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
            .WithHttpEndpoint(port: httpPort, targetPort: 9411, name: "http")
            .WithEndpoint(targetPort: 4317, name: "otlp-grpc", scheme: "http")
            .WithEndpoint(targetPort: 4318, name: "otlp-http", scheme: "http");
    }
}
