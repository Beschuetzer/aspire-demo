# Aspire Demo

A .NET Aspire demo application showcasing a distributed system with a web frontend, API service, and a full observability stack.

## Architecture

```
┌─────────────────┐     ┌─────────────────┐
│   webfrontend   │────▶│   apiservice    │
│  (Blazor App)   │     │   (Minimal API) │
└────────┬────────┘     └────────┬────────┘
         │                       │
         └───────────┬───────────┘
                     ▼
           ┌─────────────────┐
           │  otel-collector │
           └────────┬────────┘
                    │
     ┌──────────────┼──────────────┐
     ▼              ▼              ▼
┌─────────┐  ┌───────────┐  ┌───────────┐
│  Tempo  │  │Prometheus │  │   Loki    │
│(traces) │  │ (metrics) │  │  (logs)   │
└────┬────┘  └─────┬─────┘  └─────┬─────┘
     │             │              │
     └─────────────┼──────────────┘
                   ▼
            ┌───────────┐
            │  Grafana  │
            │(dashboards)│
            └───────────┘
```

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/products/docker-desktop) or [Podman](https://podman.io/)
- [Azure Developer CLI (azd)](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd) (for Azure deployment)

## Running Locally

### Using Aspire (Recommended)

Run the application with the Aspire dashboard:

```bash
dotnet run --project AspireDemo.AppHost
```

This starts all services including:

- **webfrontend** - Blazor web application
- **apiservice** - Backend API
- **otel-collector** - OpenTelemetry Collector
- **prometheus** - Metrics storage (<http://localhost:9090>)
- **loki** - Log aggregation (<http://localhost:3100>)
- **tempo** - Distributed tracing (<http://localhost:9411>)
- **grafana** - Dashboards and visualization (<http://localhost:3000>)

### Using Docker Compose

Generate the Docker Compose output and run:

```powershell
.\docker-publish.ps1
cd AspireDemo.AppHost\aspire-output
docker compose up
```

## Deploying to Azure via azd

### Initial Setup

1. Log in to Azure:

   ```powershell
   .\azd-deploy.ps1 -Command "auth login"
   ```

2. Initialize the environment:

   ```powershell
   .\azd-deploy.ps1 -Command "init"
   ```

3. Provision infrastructure and deploy:

   ```powershell
   .\azd-deploy.ps1 -Command "up"
   ```

### Subsequent Deployments

To deploy updates:

```powershell
.\azd-deploy.ps1 -Command "deploy"
```

To provision infrastructure changes:

```powershell
.\azd-deploy.ps1 -Command "provision"
```

### CI/CD with GitHub Actions

The repository includes a GitHub Actions workflow (`.github/workflows/azure-dev.yml`) that automatically deploys to Azure on pushes to `main`.

Required GitHub repository variables:

- `AZURE_CLIENT_ID`
- `AZURE_ENV_NAME`
- `AZURE_LOCATION`
- `AZURE_SUBSCRIPTION_ID`
- `AZURE_TENANT_ID`

## Deploying with Docker Compose

Generate the Docker Compose files for standalone deployment:

```powershell
.\docker-publish.ps1
```

This creates output in `AspireDemo.AppHost/aspire-output/` containing:

- `docker-compose.yaml` - Main compose file
- `.env.Production` - Environment variables
- Container images published to local registry

To run the generated compose file:

```bash
cd AspireDemo.AppHost/aspire-output
docker compose up -d
```

## Observability

### Grafana Dashboards

Access Grafana at <http://localhost:3000> (default credentials: admin/admin)

Pre-configured datasources:

- **Prometheus** - Metrics from applications and infrastructure
- **Loki** - Aggregated logs from all services
- **Tempo** - Distributed traces with correlation to logs

### OpenTelemetry

Both `webfrontend` and `apiservice` are configured to send telemetry to the OTEL Collector:

- **Traces** → Tempo
- **Metrics** → Prometheus (via remote write)
- **Logs** → Debug exporter (configurable)

## Project Structure

```
AspireDemo/
├── AspireDemo.AppHost/          # Aspire orchestration
│   ├── Monitoring/              # Monitoring stack configs
│   │   ├── prometheus.yml
│   │   ├── loki-config.yaml
│   │   ├── tempo-config.yaml
│   │   └── grafana-provisioning/
│   ├── OpenTelemetryCollector/  # OTEL collector resource
│   └── otel-collector-config.yaml
├── AspireDemo.ApiService/       # Backend API project
├── AspireDemo.Web/              # Blazor frontend project
├── AspireDemo.ServiceDefaults/  # Shared service configuration
├── AspireDemo.Tests/            # Integration tests
└── .github/workflows/           # CI/CD pipelines
```

## License

MIT
