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
            // This WithVolume call is commented out to avoid issues when deploying to Azure Container Apps.
            // Without volumes, data won't persist across container restarts in Azure.
            // If you need persistence in Azure, you'd need to either :
            //      Use Azure Blob Storage or Azure Files for storage backends
            //      Or configure the monitoring tools to use cloud-native storage
            //      (e.g., Loki with Azure Blob Storage)
            // .WithVolume("grafana-data", "/var/lib/grafana")
            .WithHttpEndpoint(port: port, targetPort: 3000, name: "http");
    }
}
