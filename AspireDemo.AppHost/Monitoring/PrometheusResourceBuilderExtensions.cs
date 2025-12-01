namespace AspireDemo.AppHost.Monitoring;

public static class PrometheusResourceBuilderExtensions
{
    /// <summary>
    /// Adds a Prometheus container to the application using a custom Dockerfile with baked-in config.
    /// </summary>
    public static IResourceBuilder<ContainerResource> AddPrometheus(
        this IDistributedApplicationBuilder builder,
        string name,
        int? port = null
    )
    {
        return builder
            .AddDockerfile(name, "Monitoring/prometheus")
            .WithArgs(
                "--config.file=/etc/prometheus/prometheus.yml",
                "--web.enable-remote-write-receiver"
            )
            .WithHttpEndpoint(port: port, targetPort: 9090, name: "http");
    }
}
