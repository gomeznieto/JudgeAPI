# ===== BUILD =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar y restaurar
COPY RunnerApp/RunnerApp.csproj RunnerApp/
COPY JudgeAPI/JudgeAPI.csproj JudgeAPI/
RUN dotnet restore RunnerApp/RunnerApp.csproj

# Copiar todo
COPY . .

# Limpiar los appsettings duplicados
RUN rm -f /src/JudgeAPI/appsettings.json /src/JudgeAPI/appsettings.Development.json

# Publicar binarios de RunnerApp
WORKDIR /src/RunnerApp
RUN dotnet publish -c Release -o /app/publish -p:UseAppHost=false

# ===== RUNTIME =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# Docker CLI
RUN apt-get update && \
    apt-get install -y ca-certificates curl gnupg lsb-release && \
    mkdir -m 0755 -p /etc/apt/keyrings && \
    curl -fsSL https://download.docker.com/linux/debian/gpg | gpg --dearmor -o /etc/apt/keyrings/docker.gpg && \
    echo "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/debian $(lsb_release -cs) stable" | tee /etc/apt/sources.list.d/docker.list > /dev/null && \
    apt-get update && \
    apt-get install -y docker-ce-cli && \
    rm -rf /var/lib/apt/lists/*

ENTRYPOINT ["dotnet", "RunnerApp.dll"]