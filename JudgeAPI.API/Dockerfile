# ---- build stage ----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar el csproj y restaurar
COPY JudgeAPI.csproj ./
RUN dotnet restore

# Copiar el resto del código
COPY . .
RUN dotnet publish -c Release -o /app/publish

# ---- runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Instalar g++ para compilación local
RUN apt-get update && apt-get install -y g++ && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "JudgeAPI.dll"]
