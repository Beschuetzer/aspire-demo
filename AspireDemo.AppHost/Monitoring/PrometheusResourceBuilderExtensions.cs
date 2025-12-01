namespace AspireDemo.AppHost.Monitoring;

public static class PrometheusResourceBuilderExtensions
{
    private const string PrometheusImage = "prom/prometheus";
    private const string PrometheusTag = "latest";

    /// <summary>
    /// Adds a Prometheus container to the application.
    /// </summary>
    public static IResourceBuilder<ContainerResource> AddPrometheus(
        this IDistributedApplicationBuilder builder,
        string name,
        string configFileLocation,
        int? port = null
    )
    {
        return builder
            .AddContainer(name, PrometheusImage, PrometheusTag)
            .WithArgs(
                "--config.file=/etc/prometheus/prometheus.yml",
                "--web.enable-remote-write-receiver"
            )
            .WithBindMount(configFileLocation, "/etc/prometheus/prometheus.yml", isReadOnly: true)
            .WithHttpEndpoint(port: port, targetPort: 9090, name: "http");
    }
}
