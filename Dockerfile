# syntax=docker/dockerfile:1
# NotaryPlatform API image. Build context = repo root (so backend/ paths resolve).

# ── Build stage ───────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore against just the .csproj files first — this layer is cached until a
# project's dependencies change, so day-to-day code edits skip the restore.
COPY ["backend/NotaryPlatform.API/NotaryPlatform.API.csproj",                       "NotaryPlatform.API/"]
COPY ["backend/NotaryPlatform.Application/NotaryPlatform.Application.csproj",         "NotaryPlatform.Application/"]
COPY ["backend/NotaryPlatform.Domain/NotaryPlatform.Domain.csproj",                   "NotaryPlatform.Domain/"]
COPY ["backend/NotaryPlatform.Infrastructure/NotaryPlatform.Infrastructure.csproj",   "NotaryPlatform.Infrastructure/"]
RUN dotnet restore "NotaryPlatform.API/NotaryPlatform.API.csproj"

# Copy the rest of the backend source and publish.
COPY backend/ .
RUN dotnet publish "NotaryPlatform.API/NotaryPlatform.API.csproj" \
    -c Release -o /app/publish /p:UseAppHost=false --no-restore

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app

# Non-root user, and pre-create writable dirs so a mounted volume inherits the
# correct (non-root) ownership. WebRootPath must exist for local file storage.
RUN mkdir -p /app/wwwroot/uploads /app/logs \
    && chown -R $APP_UID /app

USER $APP_UID

COPY --from=build --chown=app:app /app/publish .

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_EnableDiagnostics=0
EXPOSE 8080

# busybox wget ships in the alpine base — used by the container liveness probe.
HEALTHCHECK --interval=30s --timeout=5s --start-period=40s --retries=3 \
    CMD wget -qO- http://localhost:8080/health/live || exit 1

ENTRYPOINT ["dotnet", "NotaryPlatform.API.dll"]
