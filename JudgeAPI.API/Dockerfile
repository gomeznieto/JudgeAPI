FROM mcr.microsoft.com/dotnet/sdk:9.0

WORKDIR /src

# Copiar csproj para cachear restore
COPY ["JudgeAPI.API/JudgeAPI.API.csproj", "JudgeAPI.API/"]
COPY ["JudgeAPI.Application/JudgeAPI.Application.csproj", "JudgeAPI.Application/"]
COPY ["JudgeAPI.Domain/JudgeAPI.Domain.csproj", "JudgeAPI.Domain/"]
COPY ["JudgeAPI.Infrastructure/JudgeAPI.Infrastructure.csproj", "JudgeAPI.Infrastructure/"]

RUN dotnet restore "JudgeAPI.API/JudgeAPI.API.csproj"

EXPOSE 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

CMD ["dotnet", "watch", "--project", "JudgeAPI.API/JudgeAPI.API.csproj", "--no-launch-profile", "--no-hot-reload"]
