# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
# O usuário padrão é 'app'. Usar o nome diretamente é mais seguro.
USER app
WORKDIR /app

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/QrVision.Worker/QrVision.Worker.csproj", "src/QrVision.Worker/"]
RUN dotnet restore "./src/QrVision.Worker/QrVision.Worker.csproj"
COPY . .
WORKDIR "/src/src/QrVision.Worker"
RUN dotnet build "./QrVision.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./QrVision.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
# Troca para o usuário root temporariamente para instalar pacotes e criar a pasta
USER root
RUN apt-get update && apt-get install -y ffmpeg && rm -rf /var/lib/apt/lists/* && \
    # --- ALTERAÇÃO PRINCIPAL AQUI ---
    # Cria o diretório para o volume e define o usuário 'app' como proprietário
    mkdir /app/videos && chown app:app /app/videos

# Retorna para o usuário 'app' por segurança antes de executar a aplicação
USER app
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QrVision.Worker.dll"]