# ---- build stage ----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 1. Copiar los archivos de proyecto para cachear restore
COPY ["JudgeAPI.API/JudgeAPI.API.csproj", "JudgeAPI.API/"]
COPY ["JudgeAPI.Application/JudgeAPI.Application.csproj", "JudgeAPI.Application/"]
COPY ["JudgeAPI.Domain/JudgeAPI.Domain.csproj", "JudgeAPI.Domain/"]
COPY ["JudgeAPI.Infrastructure/JudgeAPI.Infrastructure.csproj", "JudgeAPI.Infrastructure/"]

# 2. Restaurar dependencias
RUN dotnet restore "JudgeAPI.API/JudgeAPI.API.csproj"

# 3. Copiar todo el código y publicar
COPY . .
WORKDIR "/src/JudgeAPI.API"
RUN dotnet publish "JudgeAPI.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ---- runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Herramientas requeridas para ejecución
RUN apt-get update && apt-get install -y g++ && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "JudgeAPI.API.dll"]
