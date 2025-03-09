# Estágio base para a imagem final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Estágio de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar apenas os arquivos .csproj para restaurar as dependências
COPY ["./RegisterConsumer/RegisterConsumer.csproj", "./RegisterConsumer/"]
COPY ["./Gateway_API/Gateway_API.csproj", "./Gateway_API/"]

# Restaurar dependências
RUN dotnet restore "./RegisterConsumer/RegisterConsumer.csproj"
RUN dotnet restore "./Gateway_API/Gateway_API.csproj"

# Copiar o restante dos arquivos (exceto os ignorados pelo .dockerignore)
COPY . .

# Build do projeto RegisterConsumer
RUN dotnet build "./RegisterConsumer/RegisterConsumer.csproj" -c Release -o /app/build

# Estágio de publicação
FROM build AS publish
# Publicar o projeto RegisterConsumer
RUN dotnet publish "./RegisterConsumer/RegisterConsumer.csproj" -c Release -o /app/publish

# Estágio final
FROM base AS final
WORKDIR /app
# Copiar apenas os arquivos publicados
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RegisterConsumer.dll"]