# syntax=docker/dockerfile:1
# Imagen de la API ASP.NET Core / .NET 10
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Primero solo los .csproj para aprovechar la cache de NuGet
COPY ["PRESENTATION/PRESENTATION.csproj", "PRESENTATION/"]
COPY ["APLICATION/APLICATION.csproj", "APLICATION/"]
COPY ["DOMAIN/DOMAIN.csproj", "DOMAIN/"]
COPY ["INFRASTRUCTURE/INFRASTRUCTURE.csproj", "INFRASTRUCTURE/"]
RUN dotnet restore "PRESENTATION/PRESENTATION.csproj"

COPY . .
RUN dotnet publish "PRESENTATION/PRESENTATION.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# El puerto 8080 es el que usan Render, Railway y el propio contenedor.
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish .
COPY docker-entrypoint.sh /usr/local/bin/docker-entrypoint.sh
# Convierte finales de linea Windows (CRLF) a Linux y da permisos de ejecucion.
RUN sed -i 's/\r$//' /usr/local/bin/docker-entrypoint.sh \
    && chmod +x /usr/local/bin/docker-entrypoint.sh

ENTRYPOINT ["/usr/local/bin/docker-entrypoint.sh"]
