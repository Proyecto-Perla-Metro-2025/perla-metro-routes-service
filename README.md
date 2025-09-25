# Perla Metro - Routes Service 

## 1. Descripción del Servicio
Este servicio es un componente de la arquitectura SOA de Perla Metro y es responsable de gestionar toda la lógica de negocio relacionada con las rutas de los trenes. Sus responsabilidades incluyen la creación, lectura, actualización y eliminación de rutas, así como el manejo de las estaciones y sus relaciones en un modelo de grafo.

---

## 2. Arquitectura y Patrón de Diseño
El proyecto sigue los principios de la **Arquitectura Limpia (Clean Architecture)** para garantizar una clara separación de responsabilidades:
- **Core**: Contiene las entidades y las interfaces (los contratos).
- **Application**: Contiene la lógica de negocio y orquestación (servicios y mapeo).
- **Infrastructure**: Se encarga de la comunicación con la base de datos Neo4j.
- **Controllers (API)**: Expone la funcionalidad a través de una API RESTful.

A nivel de sistema, este servicio forma parte de un **Monolito Distribuido con Arquitectura Orientada a Servicios (SOA)**.

---

## 3. Configuración del Entorno Local (Paso a Paso)
Esta guía detalla cómo configurar el entorno para ejecutar el proyecto en una máquina local.

### Prerrequisitos
Asegúrate de tener instalado lo siguiente:
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Una instancia de base de datos Neo4j (se recomienda [AuraDB Free Tier](https://neo4j.com/cloud/aura-free/)).

### Pasos de Configuración
1.  **Clonar el Repositorio**:
    ```bash
    git clone https://github.com/Proyecto-Perla-Metro-2025/perla-metro-routes-service
    cd perla-metro-routes-service
    ```

2.  **Configurar las Credenciales de la Base de Datos**:
    Este proyecto lee las credenciales desde un archivo `appsettings.Development.json` que no está incluido en el repositorio por seguridad. Debes crearlo a partir de la plantilla.

    * **Navega a la carpeta del proyecto**:
        ```bash
        cd RoutesService
        ```
    * **Copia la plantilla**:
        ```bash
        # En Windows (Command Prompt)
        copy appsettings.Development.template.json appsettings.Development.json

        # En Windows (PowerShell)
        cp appsettings.Development.template.json appsettings.Development.json

        # En Linux / macOS
        cp appsettings.Development.template.json appsettings.Development.json
        ```
    * **Edita el archivo `appsettings.Development.json`**: Abre el archivo recién creado y reemplaza los placeholders con tus credenciales de Neo4j Aura.

        **El archivo debe quedar así:**
        ```json
        {
          "Neo4j": {
            "Uri": "neo4j+s://xxxxxxxx.databases.neo4j.io",
            "User": "neo4j",
            "Password": "TU_CONTRASENA_SECRETA"
          }
        }
        ```

3.  **Restaurar Paquetes NuGet**:
    Desde la carpeta `RoutesService`, ejecuta el siguiente comando para descargar todas las dependencias del proyecto.
    ```bash
    dotnet restore
    ```

---

## 4. Cómo Ejecutar el Proyecto Localmente
Una vez configurado el entorno, puedes ejecutar la API.

1.  Asegúrate de estar en la carpeta del proyecto: `perla-metro-routes-service/RoutesService`.
2.  Ejecuta el siguiente comando:
    ```bash
    dotnet run
    ```
3.  La terminal te mostrará un mensaje confirmando que el servidor está en funcionamiento:
    ```
    info: Microsoft.Hosting.Lifetime[14]
          Now listening on: http://localhost:8080
    ```
4.  La API ya está disponible en `http://localhost:8080`.
5.  Puedes acceder a la documentación interactiva de Swagger en `http://localhost:8080/swagger`.

---

## 5. Guía de Uso de la API (Endpoints)
A continuación se detalla cómo consumir cada endpoint usando una herramienta como cURL o Postman.

### 5.1 Crear una Nueva Ruta
* **Endpoint**: `POST /api/routes`
* **Descripción**: Crea una nueva ruta y sus conexiones con las estaciones en el grafo.
* **Body (JSON)**:
  ```json
  {
    "originStation": "Estación A",
    "destinationStation": "Estación C",
    "startTime": "2025-11-10T09:00:00Z",
    "endTime": "2025-11-10T09:30:00Z",
    "intermediateStops": [ "Estación B" ],
    "isActive": true
  }
### 5.2 Obtener Todas las Rutas
* **Endpoint**: `GET /api/routes`
* **Descripción**: Devuelve un listado de todas las rutas. Se puede filtrar por estado.
* No necestia de un body
### 5.3 Obtener una Ruta por ID
* **Endpoint**: `GET /api/routes/{id}`
* **Descripción**: Devuelve los detalles de una ruta específica. No incluye paradas intermedias.
* Ejemplo con cURL (reemplaza {id} con un ID real):
  ```bash
  curl http://localhost:8080/api/routes/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
### 5.4 Actualizar una Ruta
* **Endpoint**: `PUT /api/routes/{id}`
* **Descripción**:  Modifica una ruta existente.
* **Body (JSON)**:
  ```json
  {
    "originStation": "Estación A-Modificada",
    "destinationStation": "Estación D",
    "startTime": "2025-11-10T10:00:00",
    "endTime": "2025-11-10T10:45:00",
    "intermediateStops": [ "Estación X", "Estación Y" ]
  }
### 5.5 Desactivar una Ruta (Soft Delete)
* **Endpoint**: `DELETE /api/routes/{id}`
* **Descripción**: Marca una ruta como inactiva.
* **Ejemplo con cURL:**:
  ```bash
  curl -X DELETE http://localhost:8080/api/routes/{id}
