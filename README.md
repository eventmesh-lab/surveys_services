# Surveys Services - Microservicio de Encuestas de Satisfacción

## Descripción

Microservicio encargado de gestionar encuestas de satisfacción para eventos. Permite que los usuarios que han pagado y asistido a eventos puedan responder encuestas automáticas de satisfacción, y proporciona estadísticas agregadas de las respuestas.

### Problema de Negocio que Resuelve
- **Recopilación automatizada de feedback:** Genera encuestas automáticas para eventos finalizados sin intervención manual.
- **Validación de elegibilidad:** Solo permite responder encuestas a usuarios que efectivamente pagaron por el evento.
- **Análisis de satisfacción:** Calcula promedios y estadísticas de las respuestas para medir la calidad de los eventos.
- **Prevención de duplicados:** Evita que un usuario responda la misma encuesta múltiples veces.

---

## Tabla de Contenidos

- [Arquitectura y Flujo de Datos](docs/architecture.md)
- [API - Endpoints y Contratos](docs/api.md)
- [Configuración y Setup](docs/setup.md)

---

## Stack Tecnológico

- **.NET 8.0** - Framework principal
- **ASP.NET Core Web API** - Capa de presentación
- **PostgreSQL 15** - Base de datos relacional
- **Entity Framework Core 9.0** - ORM
- **MediatR 14.0** - Patrón CQRS (Command Query Responsibility Segregation)
- **Docker & Docker Compose** - Containerización
- **Swagger/OpenAPI** - Documentación interactiva de API

---

## Quick Start

### Usando Docker Compose (Recomendado)

```bash
docker-compose up
```

El servicio estará disponible en: `http://localhost:7186`  
Swagger UI: `http://localhost:7186/swagger`

### Desarrollo Local

```bash
# Restaurar dependencias
dotnet restore

# Aplicar migraciones (requiere PostgreSQL corriendo)
dotnet ef database update --project src/surveys_services.infrastructure

# Ejecutar el servicio
dotnet run --project src/surveys_services.api
```

---

## Estructura del Proyecto

```
surveys_services/
├── src/
│   ├── surveys_services.api/          # Controladores y configuración API
│   ├── surveys_services.application/  # Casos de uso (Commands/Queries)
│   ├── surveys_services.domain/       # Entidades y lógica de negocio
│   └── surveys_services.infrastructure/ # Persistencia y servicios externos
├── tests/                              # Pruebas unitarias
├── docs/                               # Documentación técnica
├── Dockerfile                          # Imagen de producción
└── docker-compose.yml                  # Orquestación local
```

---

## Contacto y Contribución

Para más información sobre el proyecto, consulta los documentos en la carpeta `docs/`.
