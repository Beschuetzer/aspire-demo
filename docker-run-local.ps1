# Helper script to generate and publish Docker images with DEPLOY_TO_AZURE unset
param(
    [Parameter(Mandatory = $false)]
    [string]$outputDir = "aspire-output"
)

.\docker-publish.ps1 -outputDir $outputDir;

$repoRoot = (Get-Location).Path
$monitoringSource = Join-Path $repoRoot "monitoring"
$appHostOutput = Join-Path (Join-Path $repoRoot "AspireDemo.AppHost") $outputDir
$monitoringDest = Join-Path $appHostOutput "monitoring"
if (Test-Path $monitoringDest) {
    Remove-Item $monitoringDest -Recurse -Force -ErrorAction SilentlyContinue
}
New-Item -ItemType Directory -Path $monitoringDest -Force | Out-Null
Copy-Item -Recurse -Force $monitoringSource\* $monitoringDest

$networkName = "aspire"

if (Get-Command podman -ErrorAction SilentlyContinue) {
    if (-not (podman network exists $networkName 2>$null)) {
        podman network create $networkName | Out-Null
        Write-Host "Created podman network '$networkName'"
    }
}
elseif (Get-Command docker -ErrorAction SilentlyContinue) {
    if (-not (docker network inspect $networkName >/dev/null 2>&1)) {
        docker network create $networkName | Out-Null
        Write-Host "Created docker network '$networkName'"
    }
}

docker compose -f .\AspireDemo.AppHost\$outputDir\docker-compose.yaml -f .\monitoring\docker-compose-monitoring.yaml --env-file .\AspireDemo.AppHost\$outputDir\.env.Production up