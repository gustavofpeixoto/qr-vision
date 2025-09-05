# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# Usando o usuário 'app' diretamente para maior robustez
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/QrVision.Api/QrVision.Api.csproj", "src/QrVision.Api/"]
RUN dotnet restore "./src/QrVision.Api/QrVision.Api.csproj"
COPY . .
WORKDIR "/src/src/QrVision.Api"
RUN dotnet build "./QrVision.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./QrVision.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
# Troca para o usuário root para criar o diretório do volume
USER root
# --- ALTERAÇÃO PRINCIPAL AQUI ---
# Cria o diretório para o volume e define o usuário 'app' como proprietário
RUN mkdir /app/videos && chown app:app /app/videos

# Retorna para o usuário 'app' por segurança
USER app
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QrVision.Api.dll"]