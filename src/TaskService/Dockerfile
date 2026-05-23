# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY TaskService.slnx .
COPY TaskService.Domain/TaskService.Domain.csproj             TaskService.Domain/
COPY TaskService.Application/TaskService.Application.csproj   TaskService.Application/
COPY TaskService.Infrastructure/TaskService.Infrastructure.csproj TaskService.Infrastructure/
COPY TaskService.API/TaskService.API.csproj                   TaskService.API/

RUN dotnet restore

COPY . .
RUN dotnet publish TaskService.API/TaskService.API.csproj \
    -c Release -o /app/publish

# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends wget \
    && rm -rf /var/lib/apt/lists/*

RUN useradd -r -s /bin/false appuser && chown -R appuser /app
USER appuser

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "TaskService.API.dll"]
