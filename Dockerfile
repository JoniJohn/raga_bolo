# ── Stage 1: Build ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo

# Copy solution and project files first for layer-cached restore
COPY Raga.slnx ./
COPY src/Domain/Raga.Domain.csproj             src/Domain/
COPY src/Application/Raga.Application.csproj   src/Application/
COPY src/Infrastructure/Raga.Infrastructure.csproj src/Infrastructure/
COPY src/Api/Raga.Api.csproj                   src/Api/

RUN dotnet restore src/Api/Raga.Api.csproj

# Copy the rest of the source and publish
COPY src/ src/
RUN dotnet publish src/Api/Raga.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

# ── Stage 2: Runtime ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Install curl for healthchecks and create non-root user for security
RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/* \
    && groupadd -r appgroup && useradd -r -g appgroup appuser

USER appuser

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Raga.Api.dll"]
