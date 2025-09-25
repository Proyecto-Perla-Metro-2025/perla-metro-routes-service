# --- Etapa 1: Build (Construcción) ---
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copia los archivos de solución y proyecto.
COPY *.sln .
COPY RoutesService/*.csproj ./RoutesService/

# Restaura los paquetes NuGet.
RUN dotnet restore

# Copia todo el resto del código fuente.
COPY . .

# Establece el directorio de trabajo en la carpeta del proyecto.
WORKDIR /app/RoutesService

# Publica la aplicación.
RUN dotnet publish -c Release -o /app/publish


# --- Etapa 2: Final (Ejecución) ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# --- AÑADIR USUARIO NO-ROOT POR SEGURIDAD ---
# Crea un grupo y un usuario con permisos limitados.
RUN addgroup --system --gid 1001 appgroup \
    && adduser --system --uid 1001 --gid 1001 --shell /bin/false appuser

# Copia solo los archivos publicados de la etapa de 'build'.
COPY --from=build /app/publish .

# Cambia el propietario de los archivos al nuevo usuario.
RUN chown -R appuser:appgroup /app
# Cambia al nuevo usuario para que la aplicación no se ejecute como root.
USER appuser

# --- EXPONER PUERTO Y AÑADIR HEALTH CHECK ---
# Documenta el puerto que la aplicación usa.
EXPOSE 8080

# Define una comprobación de salud para que Docker sepa si la API está funcionando.
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Define el punto de entrada para ejecutar la aplicación.
ENTRYPOINT ["dotnet", "RoutesService.dll"]