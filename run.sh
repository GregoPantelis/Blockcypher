#!/bin/bash
set -e

ROOT_DIR="$(cd "$(dirname "$0")" && pwd)"
ENV_FILE="$ROOT_DIR/.env"
PROJECT="$ROOT_DIR/src/API/ICMarkets.Blockcypher.Api/ICMarkets.Blockcypher.Api.csproj"

if [ ! -f "$ENV_FILE" ]; then
    echo "Error: .env file not found."
    exit 1
fi

set -a
source "$ENV_FILE"
set +a

export ASPNETCORE_ENVIRONMENT="$ASPNETCORE_ENVIRONMENT"
export InitialUser__Username="$INITIAL_USER_USERNAME"
export InitialUser__Password="$INITIAL_USER_PASSWORD"

has_docker() {
    command -v docker >/dev/null 2>&1 &&
    docker compose version >/dev/null 2>&1 &&
    docker info >/dev/null 2>&1
}

has_dotnet8() {
    command -v dotnet >/dev/null 2>&1 &&
    dotnet --list-sdks 2>/dev/null | grep -q '^8\.'
}

run_docker() {
    if ! has_docker; then
        echo "Error: Docker with Docker Compose is not available."
        exit 1
    fi

    echo "Starting Blockcypher API using Docker..."
    echo "Environment: $ASPNETCORE_ENVIRONMENT"

    cd "$ROOT_DIR"
    docker compose up --build
}

run_dotnet() {
    if ! has_dotnet8; then
        echo "Error: .NET 8 SDK is not installed."
        exit 1
    fi

    echo "Starting Blockcypher API using .NET 8..."
    echo "Environment: $ASPNETCORE_ENVIRONMENT"

    dotnet run \
        --project "$PROJECT" \
        --urls "http://localhost:8080"
}

case "${1:-auto}" in
    docker)
        run_docker
        ;;

    dotnet)
        run_dotnet
        ;;

    auto)
        if has_docker; then
            echo "Docker detected."
            run_docker
        elif has_dotnet8; then
            echo "Docker not available. .NET 8 SDK detected."
            run_dotnet
        else
            echo "Error: Cannot run Blockcypher API."
            echo "Install either:"
            echo "  - Docker with Docker Compose"
            echo "  - .NET 8 SDK"
            exit 1
        fi
        ;;

    *)
        echo "Usage: $0 [docker|dotnet]"
        echo ""
        echo "  docker  Run using Docker Compose"
        echo "  dotnet  Run using .NET 8 SDK"
        echo "  no arg  Automatically select Docker or .NET 8"
        exit 1
        ;;
esac