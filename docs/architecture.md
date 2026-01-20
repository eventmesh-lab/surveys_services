# Arquitectura del Microservicio de Encuestas

## Patrón Arquitectónico

Este microservicio implementa **Clean Architecture** con **CQRS (Command Query Responsibility Segregation)** utilizando el patrón Mediator.

### Capas de la Aplicación

```
┌─────────────────────────────────────────┐
│  surveys_services.api (Controllers)     │  ← HTTP/REST
├─────────────────────────────────────────┤
│  surveys_services.application           │  ← Lógica de Aplicación
│  - Commands (Escritura)                 │
│  - Queries (Lectura)                    │
│  - DTOs                                  │
├─────────────────────────────────────────┤
│  surveys_services.domain                │  ← Lógica de Negocio
│  - Entities                             │
│  - Interfaces                           │
│  - Enums & Constants                    │
├─────────────────────────────────────────┤
│  surveys_services.infrastructure        │  ← Implementaciones
│  - Repositories (PostgreSQL)            │
│  - External Services (HTTP Clients)     │
│  - Mappers                              │
└─────────────────────────────────────────┘
```

---

## Flujo de Datos

### Escenario 1: Usuario consulta encuestas pendientes

**Endpoint:** `GET /api/surveys/pendientes/{email}`

```
1. Controller recibe la petición HTTP
   ↓
2. Crea GetPendingSurveysByUserQuery
   ↓
3. MediatR enruta al Handler correspondiente
   ↓
4. Handler ejecuta la lógica:
   a. Llama a UserService → Microservicio de Usuarios (obtener ID por email)
   b. Llama a PagosService → Microservicio de Pagos (obtener eventos pagados)
   c. Por cada evento pagado:
      - Llama a EventosService → Microservicio de Eventos (verificar estado "Publicado")
      - Si el evento está publicado y no existe encuesta → la crea automáticamente
      - Consulta SurveysRepository (PostgreSQL) → verificar si el usuario ya respondió
   d. Filtra encuestas no respondidas
   ↓
5. Handler retorna List<PendingSurveyDto>
   ↓
6. Controller serializa a JSON y devuelve HTTP 200
```

### Escenario 2: Usuario registra una respuesta

**Endpoint:** `POST /api/surveys/registerRespuesta`

```
1. Controller recibe RegisterAnswerDto (JSON)
   ↓
2. Crea RegisterAnswerCommand
   ↓
3. MediatR enruta al Handler correspondiente
   ↓
4. Handler ejecuta validaciones:
   a. Valida que el valor esté entre 1-5 (enum)
   b. Llama a UserService → obtener ID del usuario
   c. Consulta AnswerRepository → verificar que no haya respondido antes
   d. Si es válido → crea entidad Answer y la persiste
   ↓
5. Handler retorna el ID de la respuesta creada
   ↓
6. Controller retorna HTTP 200 (éxito) o 400/409 (errores)
```

---

## Dependencias Externas

### 1. **Microservicio de Usuarios**
- **URL Base:** `http://localhost:7181/api/users`
- **Endpoint usado:** `GET /getIdUser/{correo}`
- **Propósito:** Obtener el GUID del usuario a partir de su email.
- **Detección:** `UserService.cs` línea 31

### 2. **Microservicio de Pagos**
- **URL Base:** `http://localhost:7183/api/payments`
- **Endpoint usado:** `GET /obtenertHistorialPagosUsuario/{correo}`
- **Propósito:** Obtener la lista de eventos por los que un usuario ha pagado.
- **Retorna:** `List<HistorialPagoExternalDto>` con propiedad `IdEvento`.
- **Detección:** `PagosService.cs` líneas 27-29

### 3. **Microservicio de Eventos**
- **URL Base:** `http://localhost:5000/api/Eventos` *(configurado en Program.cs como 5002, pero usado como 5000 en el código)*
- **Endpoint usado:** `GET /{eventoId}`
- **Propósito:** Obtener el estado de un evento (Publicado, Finalizado, etc.).
- **Retorna:** `EventoDto` con propiedades `Id`, `Nombre`, `Estado`.
- **Detección:** `EventosService.cs` línea 44

### 4. **Base de Datos PostgreSQL**
- **Host:** `localhost` (desarrollo) / `db` (docker-compose)
- **Puerto:** `5432`
- **Base de Datos:** `surveys-service`
- **ORM:** Entity Framework Core con Npgsql
- **Tablas:**
  - `Surveys` → Encuestas
  - `Questions` → Preguntas de cada encuesta
  - `Answers` → Respuestas de usuarios

---

## Modelo de Datos

### Entidades Principales

#### **Survey (Encuesta)**
```csharp
- Id: Guid              // Identificador único
- EventoId: Guid        // Referencia al evento (FK lógica a microservicio externo)
- Titulo: string        // Título de la encuesta
- FechaCreacion: DateTime
```

#### **Question (Pregunta)**
```csharp
- Id: Guid              // Identificador único
- IdEncuesta: Guid      // FK a Survey
- Text: string          // Texto de la pregunta
```
**Preguntas predefinidas** (SurveyConstants.cs):
1. "¿Cómo calificaría la organización del evento?"
2. "¿Qué le pareció el contenido presentado?"
3. "¿Recomendaría este evento a un colega?"

#### **Answer (Respuesta)**
```csharp
- Id: Guid              // Identificador único
- PreguntaId: Guid      // FK a Question
- UsuarioId: Guid       // Referencia al usuario (FK lógica a microservicio externo)
- Valor: EnumValue      // Escala de satisfacción (1-5)
- FechaRespuesta: DateTime
```

**EnumValue:**
```csharp
1 = mediocre
2 = malo
3 = regular
4 = excelente
5 = extraordinario
```

### Relaciones
```
Survey (1) ──→ (N) Question
Question (1) ──→ (N) Answer
```

---

## Patrón CQRS Implementado

### Commands (Escritura)
- **RegisterAnswerCommand** → Registrar una respuesta de usuario
  - Handler: `RegisterAnswerHandler`

### Queries (Lectura)
1. **GetPendingSurveysByUserQuery** → Encuestas pendientes por usuario
2. **GetUserSurveyAnswersByEventQuery** → Respuestas de un usuario en un evento
3. **GetDetailSurveyAndQuestionQuery** → Detalle de una encuesta y sus preguntas
4. **PromedioEncuestaPorEventoQuery** → Promedio de respuestas por evento
5. **GetCompletedSurveysByUserQuery** → Encuestas completadas por usuario

---

## Deuda Técnica Detectada

### 🔴 Crítico

1. **URLs hardcodeadas en servicios externos**
   - **Ubicación:** `UserService.cs`, `PagosService.cs`, `EventosService.cs`
   - **Problema:** Las URLs de los microservicios están en el código fuente (`http://localhost:7181`, `http://localhost:7183`, `http://localhost:5000`).
   - **Impacto:** No se puede desplegar en otros ambientes sin recompilar.
   - **Solución:** Mover a configuración (`appsettings.json`).

2. **Inconsistencia en configuración de EventosService**
   - **Ubicación:** `Program.cs` línea 36 vs `EventosService.cs` línea 44
   - **Problema:** Se configura `http://localhost:5002` en Program.cs pero se usa `http://localhost:5000` en el código.
   - **Impacto:** El HttpClient configurado nunca se usa, se sobrescribe con URL hardcodeada.

### 🟡 Medio

3. **Console.WriteLine en producción**
   - **Ubicación:** `EventosService.cs` (líneas 50, 52, 70), `PagosService.cs` (línea 37), `UserService.cs` (línea 39), handlers múltiples
   - **Problema:** Uso de `Console.WriteLine` en lugar de `ILogger`.
   - **Impacto:** Dificulta debugging en producción, no se integra con sistemas de logs.

4. **Mock comentado en EventosService**
   - **Ubicación:** `EventosService.cs` líneas 24-38, línea 60
   - **Problema:** Código muerto/comentado con datos mock.
   - **Impacto:** Confunde al mantenimiento futuro.
   - **Solución:** Eliminar o mover a capa de pruebas.

5. **Swallowing de excepciones**
   - **Ubicación:** `GetPendingSurveysByUserHandler.cs` línea 100-103
   - **Problema:** Se capturan todas las excepciones con `catch (Exception)` y se hace `continue` sin logear.
   - **Impacto:** Errores silenciosos, dificulta debugging.

6. **Manejo inconsistente de errores**
   - **Ubicación:** `EventosService.cs` línea 74
   - **Problema:** Se lanza `ArgumentException` con mensaje genérico al obtener un evento.
   - **Impacto:** No se distingue entre error de red, timeout o respuesta 404.

### 🟢 Bajo

7. **CORS abierto solo para localhost:3000**
   - **Ubicación:** `Program.cs` líneas 15-23
   - **Problema:** Hardcoded para desarrollo local.
   - **Solución:** Configurar dinámicamente según ambiente.

8. **Migraciones automáticas en startup**
   - **Ubicación:** `Program.cs` líneas 67-82
   - **Problema:** Las migraciones se aplican automáticamente al iniciar la app.
   - **Riesgo:** En producción puede causar downtime si hay migraciones complejas.
   - **Recomendación:** Aplicar migraciones como paso previo al despliegue.

9. **Falta de paginación**
   - **Ubicación:** Todos los endpoints que retornan listas
   - **Problema:** No hay límites en las respuestas.
   - **Impacto:** Potencial problema de rendimiento si un usuario tiene muchas encuestas.

10. **Falta validación de entrada completa**
    - **Ubicación:** `SurveysController.cs`
    - **Problema:** No hay atributos de validación en DTOs (ej: `[Required]`, `[EmailAddress]`).
    - **Solución:** Implementar FluentValidation o DataAnnotations.

---

## Consideraciones de Seguridad

- ✅ **No hay credenciales hardcodeadas** en el código.
- ⚠️ **Falta autenticación/autorización:** Cualquiera puede llamar los endpoints con un email.
- ⚠️ **Falta rate limiting:** Vulnerable a abuso.
- ⚠️ **HTTPS no forzado:** En desarrollo se usa HTTP (línea 84 de Program.cs intenta redirigir, pero configuración no lo soporta).
