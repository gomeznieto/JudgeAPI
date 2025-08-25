# Imagen base con .NET SDK para desarrollo (incluye dotnet watch)
FROM mcr.microsoft.com/dotnet/sdk:8.0

# Directorio de trabajo dentro del contenedor
WORKDIR /app

# Copiamos todo el contenido del proyecto (csproj + src)
COPY . .

# Restauramos los paquetes
RUN dotnet restore
