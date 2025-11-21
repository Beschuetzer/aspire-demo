namespace MetricsApp.AppHost.OpenTelemetryCollector;

internal static class DevCertHostingExtensions
{
    public static void RunWithHttpsDevCertificate(
        IResourceBuilder<OpenTelemetryCollectorResource> resourceBuilder,
        string certFileEnvVar,
        string certKeyEnvVar,
        Action<string, string> configureArgs)
    {
        // Get the development certificate paths
        var certPath = GetDevCertificatePath();
        var keyPath = GetDevCertificateKeyPath();

        if (!string.IsNullOrEmpty(certPath) && !string.IsNullOrEmpty(keyPath))
        {
            resourceBuilder
                .WithBindMount(certPath, "/dev-certs/dev-cert.pem", isReadOnly: true)
                .WithBindMount(keyPath, "/dev-certs/dev-cert.key", isReadOnly: true);

            configureArgs(certPath, keyPath);
        }
    }

    private static string? GetDevCertificatePath()
    {
        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var certPath = Path.Combine(userProfile, ".aspnet", "dev-certs", "https", "aspnetapp.pem");
        
        return File.Exists(certPath) ? certPath : null;
    }

    private static string? GetDevCertificateKeyPath()
    {
        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var keyPath = Path.Combine(userProfile, ".aspnet", "dev-certs", "https", "aspnetapp.key");
        
        return File.Exists(keyPath) ? keyPath : null;
    }
}
