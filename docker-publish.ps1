# Helper script to generate and publish Docker images with DEPLOY_TO_AZURE unset
param(
    [Parameter(Mandatory = $true)]
    [string]$outputDir
)

$env:DEPLOY_TO_AZURE = "false"
Invoke-Expression "aspire publish -o $outputDir"
