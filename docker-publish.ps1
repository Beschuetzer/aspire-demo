# Helper script to generate and publish Docker images with DEPLOY_TO_AZURE unset
param(
    [Parameter(Mandatory = $false)]
    [string]$outputDir = "aspire-output"
)

$env:DEPLOY_TO_AZURE = "false"
Invoke-Expression "aspire do prepare-env"
