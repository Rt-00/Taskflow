{ pkgs ? import <nixpkgs> {} }:

pkgs.mkShell {
  name = "taskflow-dev";

  packages = with pkgs; [
    # Runtime e SDK
    dotnet-sdk_10

    # Ferramentas de banco
    postgresql_16
    redis

    # Containers
    docker
    docker-compose

    # Utilitários
    curl
    jq
    git
  ];

  shellHook = ''
    echo ""
    echo "  TaskFlow dev environment"
    echo "  .NET $(dotnet --version)"
    echo "  Docker $(docker --version | cut -d' ' -f3 | tr -d ',')"
    echo ""

    export DOTNET_CLI_TELEMETRY_OPTOUT=1
    export ASPNETCORE_ENVIRONMENT=Development
  '';
}
