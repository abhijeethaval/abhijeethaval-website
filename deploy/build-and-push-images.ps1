$ErrorActionPreference = "Stop"

# Configuration variables
$APP_NAME = "abhijeet-site"
$LOCATION = "centralindia"
$RESOURCE_GROUP = "rg-abhijeet-site"
$ACR_NAME = "acrabhijeetsite"
$ACA_ENV = "aca-env-abhijeet-site"
$IDENTITY_NAME = "aca-id-abhijeet-site"
$API_APP = "abhijeetsite-api"
$WEB_APP = "abhijeetsite-web"

# Add Azure CLI path if not present
if (-not (Get-Command "az" -ErrorAction SilentlyContinue)) {
    $azureCliPath = "C:\Program Files\Microsoft SDKs\Azure\CLI2\wbin"
    if (Test-Path $azureCliPath) {
        $env:PATH = "$azureCliPath;$env:PATH"
    }
}

# Determine repository root relative to script location
$REPO_ROOT = Split-Path -Parent $PSScriptRoot

# Determine tag
$TAG = $args[0]
if ([string]::IsNullOrEmpty($TAG)) {
    try {
        # Run git from repo root
        $gitSha = (git -C $REPO_ROOT rev-parse --short HEAD 2>$null)
        if ($LASTEXITCODE -eq 0 -and $gitSha) {
            $TAG = $gitSha.Trim()
        }
    } catch {
        $TAG = ""
    }
}

if ([string]::IsNullOrEmpty($TAG)) {
    Write-Warning "git short SHA could not be automatically determined. Using tag '1.0.0'."
    $TAG = "1.0.0"
}

Write-Host "Using image tag: $TAG"

# Format ACR Login Server deterministically to bypass local network lookup issues
$ACR_LOGIN_SERVER = "${ACR_NAME}.azurecr.io"
Write-Host "ACR Login Server: $ACR_LOGIN_SERVER"

# Helper function to run az acr build with resilient verification fallback
function Run-ResilientAcrBuild {
    param (
        [string]$ImageName,
        [string]$ContextPath,
        [string]$DockerfilePath = $null
    )

    Write-Host "Building and pushing $ImageName image via az acr build..."
    
    $buildError = $null
    try {
        if ($DockerfilePath) {
            az acr build `
              --registry $ACR_NAME `
              --resource-group $RESOURCE_GROUP `
              --image "${ImageName}:${TAG}" `
              --file $DockerfilePath `
              $ContextPath
        } else {
            az acr build `
              --registry $ACR_NAME `
              --resource-group $RESOURCE_GROUP `
              --image "${ImageName}:${TAG}" `
              $ContextPath
        }
    } catch {
        $buildError = $_
    }

    if ($LASTEXITCODE -ne 0 -or $buildError) {
        Write-Warning "az acr build for $ImageName completed with an exit code of $LASTEXITCODE or error. Checking if the image was successfully built and pushed in the cloud anyway..."
        
        # Wait a few seconds for registry indexing to be reliable
        Start-Sleep -Seconds 5
        
        # Query repository tags from Azure CLI (which uses the ARM Management Plane)
        $tags = (az acr repository show-tags --name $ACR_NAME --repository $ImageName --output json 2>$null | ConvertFrom-Json)
        
        if ($tags -and ($tags -contains $TAG)) {
            Write-Host "Verification successful: Image ${ImageName}:${TAG} exists in the registry! Proceeding..."
        } else {
            Write-Error "Error: $ImageName image build and push failed, and the tag was not found in the registry."
            if ($buildError) { Write-Error $buildError }
            exit 1
        }
    }
}

# 1. Build and push API image via az acr build (Docker-less on local machine, built in the cloud)
$API_DOCKERFILE_PATH = Join-Path $REPO_ROOT "src/AbhijeetSite.Api/Dockerfile"
Run-ResilientAcrBuild -ImageName $API_APP -ContextPath $REPO_ROOT -DockerfilePath $API_DOCKERFILE_PATH

# 2. Build and push Web image via az acr build (Docker-less on local machine, built in the cloud)
$WEB_SRC_PATH = Join-Path $REPO_ROOT "src/AbhijeetSite.Web"
Run-ResilientAcrBuild -ImageName $WEB_APP -ContextPath $WEB_SRC_PATH

Write-Host "Images built and pushed successfully!"
