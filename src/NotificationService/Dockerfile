# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY NotificationService.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish NotificationService.csproj \
    -c Release -o /app/publish

# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN useradd -r -s /bin/false appuser && chown -R appuser /app
USER appuser

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "NotificationService.dll"]
