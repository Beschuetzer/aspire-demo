# Helper script to run azd commands with DEPLOY_TO_AZURE set
param(
    [Parameter(Mandatory = $true)]
    [string]$Command
)

$env:DEPLOY_TO_AZURE = "true"
Invoke-Expression "azd $Command"
