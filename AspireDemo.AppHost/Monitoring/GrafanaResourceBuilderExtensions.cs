namespace AspireDemo.AppHost.Monitoring;

public static class GrafanaResourceBuilderExtensions
{
    /// <summary>
    /// Adds a Grafana container to the application for visualization and dashboards using a custom Dockerfile with baked-in provisioning.
    /// </summary>
    public static IResourceBuilder<ContainerResource> AddGrafana(
        this IDistributedApplicationBuilder builder,
        string name,
        int? port = null
    )
    {
        return builder
            .AddDockerfile(name, "Monitoring/grafana")
            .WithVolume("grafana-data", "/var/lib/grafana")
            .WithHttpEndpoint(port: port, targetPort: 3000, name: "http");
    }
}
