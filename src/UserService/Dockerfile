# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY UserService.slnx .
COPY UserService.Domain/UserService.Domain.csproj             UserService.Domain/
COPY UserService.Application/UserService.Application.csproj   UserService.Application/
COPY UserService.Infrastructure/UserService.Infrastructure.csproj UserService.Infrastructure/
COPY UserService.API/UserService.API.csproj                   UserService.API/

RUN dotnet restore

COPY . .
RUN dotnet publish UserService.API/UserService.API.csproj \
    -c Release -o /app/publish

# Stage 2: runtime (imagem final bem menor)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends wget \
    && rm -rf /var/lib/apt/lists/*

RUN useradd -r -s /bin/false appuser && chown -R appuser /app
USER appuser

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "UserService.API.dll"]
