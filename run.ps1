$ErrorActionPreference = "Stop"

$RootDir = $PSScriptRoot
$EnvFile = Join-Path $RootDir ".env"
$Project = Join-Path $RootDir "src/API/ICMarkets.Blockcypher.Api/ICMarkets.Blockcypher.Api.csproj"

if (-not (Test-Path $EnvFile)) {
    Write-Error "Error: .env file not found."
    exit 1
}

# Load .env
Get-Content $EnvFile | ForEach-Object {
    $line = $_.Trim()

    if ($line -and -not $line.StartsWith("#")) {
        $key, $value = $line -split "=", 2

        if ($key -and $value) {
            [Environment]::SetEnvironmentVariable(
                $key.Trim(),
                $value.Trim(),
                "Process"
            )
        }
    }
}

$env:InitialUser__Username = $env:INITIAL_USER_USERNAME
$env:InitialUser__Password = $env:INITIAL_USER_PASSWORD

function Test-Docker {
    if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
        return $false
    }

    docker compose version *> $null
    if ($LASTEXITCODE -ne 0) {
        return $false
    }

    docker info *> $null
    return $LASTEXITCODE -eq 0
}

function Test-DotNet8 {
    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
        return $false
    }

    $sdks = dotnet --list-sdks 2>$null
    return [bool]($sdks | Select-String "^8\.")
}

function Start-Docker {
    if (-not (Test-Docker)) {
        Write-Error "Docker with Docker Compose is not available."
        exit 1
    }

    Write-Host "Starting Blockcypher API using Docker..."
    Write-Host "Environment: $env:ASPNETCORE_ENVIRONMENT"

    Set-Location $RootDir
    docker compose up --build
}

function Start-DotNet {
    if (-not (Test-DotNet8)) {
        Write-Error ".NET 8 SDK is not installed."
        exit 1
    }

    Write-Host "Starting Blockcypher API using .NET 8..."
    Write-Host "Environment: $env:ASPNETCORE_ENVIRONMENT"

    dotnet run `
        --project $Project `
        --urls "http://localhost:8080"
}

$Mode = if ($args.Count -gt 0) { $args[0] } else { "auto" }

switch ($Mode) {
    "docker" {
        Start-Docker
    }

    "dotnet" {
        Start-DotNet
    }

    "auto" {
        if (Test-Docker) {
            Write-Host "Docker detected."
            Start-Docker
        }
        elseif (Test-DotNet8) {
            Write-Host "Docker not available. .NET 8 SDK detected."
            Start-DotNet
        }
        else {
            Write-Error @"
Cannot run Blockcypher API.

Install either:
  - Docker with Docker Compose
  - .NET 8 SDK
"@
            exit 1
        }
    }

    default {
        Write-Host "Usage: .\run.ps1 [docker|dotnet]"
        Write-Host ""
        Write-Host "  docker  Run using Docker Compose"
        Write-Host "  dotnet  Run using .NET 8 SDK"
        Write-Host "  no arg  Automatically select Docker or .NET 8"
        exit 1
    }
}