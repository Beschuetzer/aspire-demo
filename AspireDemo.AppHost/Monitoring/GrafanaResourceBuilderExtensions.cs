namespace AspireDemo.AppHost.Monitoring;

public static class GrafanaResourceBuilderExtensions
{
    private const string GrafanaImage = "grafana/grafana";
    private const string GrafanaTag = "10.1.0";

    /// <summary>
    /// Adds a Grafana container to the application for visualization and dashboards.
    /// </summary>
    public static IResourceBuilder<ContainerResource> AddGrafana(
        this IDistributedApplicationBuilder builder,
        string name,
        string provisioningPath,
        int? port = null
    )
    {
        return builder
            .AddContainer(name, GrafanaImage, GrafanaTag)
            .WithBindMount(provisioningPath, "/etc/grafana/provisioning", isReadOnly: true)
            .WithVolume("grafana-data", "/var/lib/grafana")
            .WithHttpEndpoint(port: port, targetPort: 3000, name: "http");
    }
}
