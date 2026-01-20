# Guía de Configuración y Setup

## Requisitos Previos

### Para Docker (Recomendado)
- Docker 20.10 o superior
- Docker Compose 2.0 o superior

### Para Desarrollo Local
- .NET SDK 8.0 o superior
- PostgreSQL 15 o superior
- IDE recomendado: Visual Studio 2022, Rider o Visual Studio Code

---

## Variables de Entorno

### Tabla de Variables

| Variable                             | Requerido | Default                                                          | Descripción                                    |
|--------------------------------------|-----------|------------------------------------------------------------------|------------------------------------------------|
| `ASPNETCORE_ENVIRONMENT`             | No        | `Production`                                                     | Ambiente de ejecución (Development, Staging, Production) |
| `ASPNETCORE_URLS`                    | No        | `http://*:7186`                                                  | URLs en las que escucha el servicio            |
| `ConnectionStrings__ConnectionPostgre` | Sí      | `Host=localhost;Port=5432;Database=surveys-service;Username=postgres;Password=postgres` | Cadena de conexión a PostgreSQL |

### Configuración en docker-compose.yml

```yaml
environment:
  ASPNETCORE_ENVIRONMENT: Development
  ConnectionStrings__ConnectionPostgre: "Host=db;Port=5432;Database=surveys-service;Username=postgres;Password=postgres"
```

### Configuración en appsettings.json

```json
{
  "ConnectionStrings": {
    "ConnectionPostgre": "Host=localhost;Port=5432;Database=surveys-service;Username=postgres;Password=postgres"
  }
}
```

> **⚠️ IMPORTANTE:** Las URLs de los microservicios externos están actualmente hardcodeadas en el código fuente. Esto es una deuda técnica identificada. Las URLs son:
> - Usuarios: `http://localhost:7181`
> - Pagos: `http://localhost:7183`
> - Eventos: `http://localhost:5000`

---

## Setup con Docker Compose

### Inicio Rápido

```bash
# Clonar el repositorio
git clone <repository-url>
cd surveys_services

# Levantar servicios (base de datos + API)
docker-compose up

# O en modo detached
docker-compose up -d
```

El servicio estará disponible en:
- **API:** http://localhost:7186
- **Swagger UI:** http://localhost:7186/swagger
- **PostgreSQL:** localhost:5432

### Detener Servicios

```bash
# Detener contenedores
docker-compose down

# Detener y eliminar volúmenes (⚠️ borra la base de datos)
docker-compose down -v
```

### Ver Logs

```bash
# Logs de todos los servicios
docker-compose logs -f

# Logs solo del servicio surveys
docker-compose logs -f surveys

# Logs solo de PostgreSQL
docker-compose logs -f db
```

---

## Setup Local (Sin Docker)

### 1. Instalar Dependencias

```bash
# Navegar al directorio raíz
cd surveys_services

# Restaurar paquetes NuGet
dotnet restore
```

### 2. Configurar Base de Datos PostgreSQL

**Opción A: Instalar PostgreSQL localmente**
```bash
# En Ubuntu/Debian
sudo apt-get install postgresql-15

# En macOS (con Homebrew)
brew install postgresql@15

# Iniciar servicio
sudo service postgresql start  # Linux
brew services start postgresql@15  # macOS
```

**Opción B: Usar PostgreSQL en Docker**
```bash
docker run --name surveys-postgres \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=surveys-service \
  -p 5432:5432 \
  -d postgres:15-alpine
```

### 3. Aplicar Migraciones

```bash
# Desde el directorio raíz
dotnet ef database update --project src/surveys_services.infrastructure --startup-project src/surveys_services.api

# O desde dentro de la carpeta api
cd src/surveys_services.api
dotnet ef database update
```

> **Nota:** Las migraciones también se aplican automáticamente al iniciar la aplicación (ver `Program.cs` líneas 67-82).

### 4. Ejecutar el Servicio

```bash
# Desde el directorio raíz
dotnet run --project src/surveys_services.api

# O navegar a la carpeta y ejecutar
cd src/surveys_services.api
dotnet run
```

---

## Docker - Construcción Manual

### Construir Imagen

```bash
# Desde el directorio raíz
docker build -t surveys-service:latest .

# Construir con puerto personalizado
docker build --build-arg APP_PORT=8080 -t surveys-service:latest .
```

### Ejecutar Contenedor

```bash
docker run -d \
  --name surveys-api \
  -p 7186:7186 \
  -e ConnectionStrings__ConnectionPostgre="Host=host.docker.internal;Port=5432;Database=surveys-service;Username=postgres;Password=postgres" \
  surveys-service:latest
```

> **Nota:** `host.docker.internal` permite al contenedor acceder a servicios en el host (como PostgreSQL local).

---

## Scripts y Comandos Útiles

### Comandos del Proyecto .NET

No hay scripts npm/make, pero estos son los comandos .NET más comunes:

| Comando                                  | Descripción                                    |
|------------------------------------------|------------------------------------------------|
| `dotnet restore`                         | Restaurar dependencias NuGet                   |
| `dotnet build`                           | Compilar el proyecto                           |
| `dotnet run --project src/surveys_services.api` | Ejecutar la aplicación                  |
| `dotnet test`                            | Ejecutar todas las pruebas                     |
| `dotnet watch run --project src/surveys_services.api` | Ejecutar con hot-reload            |
| `dotnet ef migrations add <NombreMigracion> --project src/surveys_services.infrastructure` | Crear nueva migración |
| `dotnet ef database update --project src/surveys_services.infrastructure` | Aplicar migraciones         |
| `dotnet publish -c Release -o ./publish` | Compilar para producción                       |

### Comandos de Entity Framework

```bash
# Listar migraciones
dotnet ef migrations list --project src/surveys_services.infrastructure --startup-project src/surveys_services.api

# Revertir última migración
dotnet ef database update <MigraciónAnterior> --project src/surveys_services.infrastructure --startup-project src/surveys_services.api

# Eliminar última migración (solo si no se ha aplicado)
dotnet ef migrations remove --project src/surveys_services.infrastructure --startup-project src/surveys_services.api

# Generar script SQL de migraciones
dotnet ef migrations script --project src/surveys_services.infrastructure --startup-project src/surveys_services.api -o migration.sql
```

---

## Configuración de CORS

**Ubicación:** `Program.cs` líneas 15-23

Actualmente configurado solo para desarrollo local:
```csharp
policy.WithOrigins("http://localhost:3000")
```

### Para permitir múltiples orígenes:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontends", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",
            "https://miapp.com",
            "https://www.miapp.com"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
```

---

## Troubleshooting

### Problema: "Failed to bind to address http://*:7186"

**Causa:** El puerto 7186 ya está en uso.

**Solución:**
```bash
# Opción 1: Cambiar puerto
export ASPNETCORE_URLS=http://*:8080
dotnet run --project src/surveys_services.api

# Opción 2: Matar proceso en el puerto
# Linux/Mac
sudo lsof -i :7186
sudo kill -9 <PID>

# Windows
netstat -ano | findstr :7186
taskkill /PID <PID> /F
```

### Problema: "Npgsql.NpgsqlException: Connection refused"

**Causa:** PostgreSQL no está corriendo o la cadena de conexión es incorrecta.

**Solución:**
```bash
# Verificar que PostgreSQL esté corriendo
docker ps | grep postgres
# O en sistema local
sudo service postgresql status

# Verificar conectividad
psql -h localhost -U postgres -d surveys-service
```

### Problema: "Unable to apply migrations"

**Causa:** Base de datos no creada o permisos insuficientes.

**Solución:**
```bash
# Crear base de datos manualmente
psql -h localhost -U postgres -c "CREATE DATABASE \"surveys-service\";"

# Luego aplicar migraciones
dotnet ef database update --project src/surveys_services.infrastructure --startup-project src/surveys_services.api
```

### Problema: "Cannot access microservices (Usuarios/Pagos/Eventos)"

**Causa:** Los otros microservicios no están corriendo o las URLs son incorrectas.

**Solución:**
- Asegurarse de que los otros microservicios estén ejecutándose en los puertos esperados
- Revisar las URLs hardcodeadas en:
  - `UserService.cs` (puerto 7181)
  - `PagosService.cs` (puerto 7183)
  - `EventosService.cs` (puerto 5000)

---

## Ambientes de Ejecución

### Development

```bash
export ASPNETCORE_ENVIRONMENT=Development
dotnet run --project src/surveys_services.api
```

- Swagger UI habilitado
- Logs detallados
- Hot reload disponible con `dotnet watch`

### Production

```bash
export ASPNETCORE_ENVIRONMENT=Production
dotnet run --project src/surveys_services.api
```

- Swagger UI deshabilitado
- Logs optimizados
- Migraciones automáticas (⚠️ considerar desactivar)

---

## Pruebas

### Ejecutar Pruebas Unitarias

```bash
# Todas las pruebas
dotnet test

# Solo un proyecto de pruebas
dotnet test tests/surveys_services.application.Tests

# Con cobertura
dotnet test /p:CollectCoverage=true /p:CoverageReporter=html
```

---

## Recursos Adicionales

- [Documentación de .NET 8.0](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [MediatR](https://github.com/jbogard/MediatR)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/15/)
