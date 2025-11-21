using Aspire.Hosting.Eventing;
using Aspire.Hosting.Lifecycle;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace MetricsApp.AppHost.OpenTelemetryCollector;

internal sealed class OpenTelemetryCollectorLifecycleHook
    : IDistributedApplicationEventingSubscriber
{
    private const string OtelExporterOtlpEndpoint = "OTEL_EXPORTER_OTLP_ENDPOINT";

    private readonly ILogger<OpenTelemetryCollectorLifecycleHook> _logger;

    public OpenTelemetryCollectorLifecycleHook(ILogger<OpenTelemetryCollectorLifecycleHook> logger)
    {
        _logger = logger;
    }

    public Task SubscribeAsync(
        IDistributedApplicationEventing eventing,
        DistributedApplicationExecutionContext executionContext,
        CancellationToken cancellationToken
    )
    {
        eventing.Subscribe<BeforeResourceStartedEvent>(BeforeResourceStartedAsync);
        return Task.CompletedTask;
    }

    private Task BeforeResourceStartedAsync(
        BeforeResourceStartedEvent @event,
        CancellationToken cancellationToken
    )
    {
        var appModel = @event.Services.GetRequiredService<DistributedApplicationModel>();
        var collectorResource = appModel
            .Resources.OfType<OpenTelemetryCollectorResource>()
            .FirstOrDefault();
        if (collectorResource == null)
        {
            _logger.LogWarning($"No {nameof(OpenTelemetryCollectorResource)} resource found.");
            return Task.CompletedTask;
        }

        var endpoint = collectorResource.GetEndpoint(
            OpenTelemetryCollectorResource.OtlpGrpcEndpointName
        );
        if (!endpoint.Exists)
        {
            _logger.LogWarning(
                $"No {OpenTelemetryCollectorResource.OtlpGrpcEndpointName} endpoint for the collector."
            );
            return Task.CompletedTask;
        }

        var resource = @event.Resource;
        if (
            !resource
                .Annotations.OfType<EnvironmentCallbackAnnotation>()
                .Any(a => a.Callback.Method.Name.Contains("OtelCollector"))
        )
        {
            resource.Annotations.Add(
                new EnvironmentCallbackAnnotation(
                    (EnvironmentCallbackContext context) =>
                    {
                        if (context.EnvironmentVariables.ContainsKey(OtelExporterOtlpEndpoint))
                        {
                            _logger.LogDebug(
                                "Forwarding telemetry for {ResourceName} to the collector.",
                                resource.Name
                            );

                            context.EnvironmentVariables[OtelExporterOtlpEndpoint] = endpoint;
                        }
                    }
                )
            );
        }

        return Task.CompletedTask;
    }
}
