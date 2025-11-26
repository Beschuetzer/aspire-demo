# Helper script to generate and publish Docker images with DEPLOY_TO_AZURE unset
param(
    [Parameter(Mandatory = $false)]
    [string]$outputDir = "aspire-output"
)

.\docker-publish.ps1 -outputDir $outputDir;
docker compose -f docker-compose.yaml -f docker-compose-monitoring.yaml --env-file .\AspireDemo.AppHost\$outputDir\.env.Production up