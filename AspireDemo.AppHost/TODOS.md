## Aspire 13 Open Telemetry Resource
I want to create an Open Telemetry Resource for Aspire 13.  Create an extension method called AddOpenTelemetryCollector, which adds the Open Telemetry Collector to the IServiceCollection.  The method should take an IConfiguration parameter to read settings from configuration.  The user should be able to configure the following settings via configuration:
	- the ports that the Open Telemetry Collector will listen on
	- the protocols that the Open Telemetry Collector will use (e.g. OTLP, Jaeger, Zipkin)
	- the exporters that the Open Telemetry Collector will use (e.g. OTLP, Jaeger, Zipkin)
The extension method should also configure the Open Telemetry Collector to use the specified settings.  The method should return the IServiceCollection to allow for method chaining.