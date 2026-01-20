# API - Documentación de Endpoints

**Base URL:** `http://localhost:7186/api/surveys`

Todos los endpoints retornan JSON. En desarrollo, Swagger UI está disponible en: `http://localhost:7186/swagger`

---

## Endpoints

### 1. Obtener Encuestas Pendientes por Usuario

**GET** `/pendientes/{email}`

Retorna todas las encuestas que el usuario debe responder. Solo incluye encuestas de eventos que:
- El usuario ha pagado (verificado en microservicio de Pagos)
- El evento está en estado "Publicado"
- El usuario aún no ha respondido

**Parámetros:**
| Nombre | Tipo   | Ubicación | Descripción                    |
|--------|--------|-----------|--------------------------------|
| email  | string | Path      | Correo electrónico del usuario |

**Respuestas:**

| Código | Descripción                     |
|--------|---------------------------------|
| 200    | Lista de encuestas pendientes   |
| 400    | Email inválido                  |
| 404    | No hay encuestas pendientes     |

**Ejemplo de Respuesta (200):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
    "eventoId": "a1b2c3d4-5678-90ab-cdef-1234567890ab",
    "titulo": "Encuesta de Satisfacción del Evento: Concierto de Rock",
    "fechaCreacion": "2024-01-15T10:30:00Z"
  },
  {
    "id": "8e7d6c5b-4a3f-2b1c-9d8e-7f6a5b4c3d2e",
    "eventoId": "f1e2d3c4-b5a6-7890-1234-567890abcdef",
    "titulo": "Encuesta de Satisfacción del Evento: Taller de Cocina",
    "fechaCreacion": "2024-01-20T14:15:00Z"
  }
]
```

---

### 2. Registrar Respuesta a una Pregunta

**POST** `/registerRespuesta`

Registra la respuesta de un usuario a una pregunta específica de una encuesta. Valida que el usuario no haya respondido previamente y que el valor esté en el rango permitido (1-5).

**Body (JSON):**
```json
{
  "encuestaId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
  "preguntaId": "9b8a7c6d-5e4f-3210-9876-543210fedcba",
  "email": "usuario@example.com",
  "valor": 5
}
```

**Campos del Body:**
| Nombre      | Tipo   | Requerido | Descripción                                          |
|-------------|--------|-----------|------------------------------------------------------|
| encuestaId  | Guid   | Sí        | ID de la encuesta                                    |
| preguntaId  | Guid   | Sí        | ID de la pregunta                                    |
| email       | string | Sí        | Correo del usuario                                   |
| valor       | int    | Sí        | Calificación (1=mediocre, 2=malo, 3=regular, 4=excelente, 5=extraordinario) |

**Respuestas:**

| Código | Descripción                                    |
|--------|------------------------------------------------|
| 200    | Respuesta registrada exitosamente              |
| 400    | Body vacío o valor fuera de rango (1-5)        |
| 409    | El usuario ya respondió esta pregunta          |
| 500    | Error interno del servidor                     |

**Ejemplo de Respuesta (200):**
```json
"b1c2d3e4-f5a6-7890-1234-567890abcdef"
```
*(Retorna el GUID del registro Answer creado. Este ID puede usarse para auditoría o referencias futuras)*

**Ejemplo de Error (409):**
```json
"El usuario usuario@example.com ya ha respondido la pregunta 9b8a7c6d-5e4f-3210-9876-543210fedcba en la encuesta 3fa85f64-5717-4562-b3fc-2c963f66afa7."
```

---

### 3. Obtener Respuestas de Usuario por Evento

**GET** `/respuestasEventoUsuario/{eventId}/{email}`

Retorna todas las respuestas que un usuario dio a la encuesta de un evento específico.

**Parámetros:**
| Nombre  | Tipo   | Ubicación | Descripción                    |
|---------|--------|-----------|--------------------------------|
| eventId | Guid   | Path      | ID del evento                  |
| email   | string | Path      | Correo electrónico del usuario |

**Respuestas:**

| Código | Descripción                          |
|--------|--------------------------------------|
| 200    | Respuestas del usuario al evento     |
| 404    | No se encontró encuesta para evento  |

**Ejemplo de Respuesta (200):**
```json
{
  "surveyId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
  "surveyTitle": "Encuesta de Satisfacción del Evento: Concierto de Rock",
  "eventId": "a1b2c3d4-5678-90ab-cdef-1234567890ab",
  "details": [
    {
      "questionId": "9b8a7c6d-5e4f-3210-9876-543210fedcba",
      "questionText": "¿Cómo calificaría la organización del evento?",
      "answerValue": 5,
      "answerText": "extraordinario",
      "answeredAt": "2024-01-15T18:45:00Z"
    },
    {
      "questionId": "1a2b3c4d-5e6f-7890-abcd-ef1234567890",
      "questionText": "¿Qué le pareció el contenido presentado?",
      "answerValue": 4,
      "answerText": "excelente",
      "answeredAt": "2024-01-15T18:46:00Z"
    }
  ]
}
```

---

### 4. Obtener Detalle de Encuesta y sus Preguntas

**GET** `/detailSurveyQuestion/{id}`

Retorna la estructura completa de una encuesta: información general y todas sus preguntas.

**Parámetros:**
| Nombre | Tipo | Ubicación | Descripción      |
|--------|------|-----------|------------------|
| id     | Guid | Path      | ID de la encuesta|

**Respuestas:**

| Código | Descripción              |
|--------|--------------------------|
| 200    | Detalle de la encuesta   |
| 404    | La encuesta no existe    |

**Ejemplo de Respuesta (200):**
```json
{
  "surveyId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
  "titulo": "Encuesta de Satisfacción del Evento: Concierto de Rock",
  "eventoId": "a1b2c3d4-5678-90ab-cdef-1234567890ab",
  "fechaCreacion": "2024-01-15T10:30:00Z",
  "questions": [
    {
      "id": "9b8a7c6d-5e4f-3210-9876-543210fedcba",
      "text": "¿Cómo calificaría la organización del evento?"
    },
    {
      "id": "1a2b3c4d-5e6f-7890-abcd-ef1234567890",
      "text": "¿Qué le pareció el contenido presentado?"
    },
    {
      "id": "2b3c4d5e-6f7a-8901-bcde-f12345678901",
      "text": "¿Recomendaría este evento a un colega?"
    }
  ]
}
```

---

### 5. Obtener Promedio de Respuestas por Evento

**GET** `/promedioRespuestasEvento/{eventId}`

Calcula el promedio de todas las respuestas dadas a la encuesta de un evento, agrupadas por pregunta.

**Parámetros:**
| Nombre  | Tipo | Ubicación | Descripción   |
|---------|------|-----------|---------------|
| eventId | Guid | Path      | ID del evento |

**Respuestas:**

| Código | Descripción                                 |
|--------|---------------------------------------------|
| 200    | Estadísticas de la encuesta                 |
| 400    | ID de evento inválido (Guid.Empty)          |
| 404    | No se encontró encuesta para el evento      |

**Ejemplo de Respuesta (200):**
```json
{
  "eventId": "a1b2c3d4-5678-90ab-cdef-1234567890ab",
  "surveyId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
  "surveyTitle": "Encuesta de Satisfacción del Evento: Concierto de Rock",
  "totalResponses": 47,
  "averagePerQuestion": [
    {
      "questionId": "9b8a7c6d-5e4f-3210-9876-543210fedcba",
      "questionText": "¿Cómo calificaría la organización del evento?",
      "averageScore": 4.2,
      "totalAnswers": 47
    },
    {
      "questionId": "1a2b3c4d-5e6f-7890-abcd-ef1234567890",
      "questionText": "¿Qué le pareció el contenido presentado?",
      "averageScore": 4.5,
      "totalAnswers": 47
    },
    {
      "questionId": "2b3c4d5e-6f7a-8901-bcde-f12345678901",
      "questionText": "¿Recomendaría este evento a un colega?",
      "averageScore": 4.8,
      "totalAnswers": 47
    }
  ],
  "overallAverage": 4.5
}
```

---

### 6. Obtener Encuestas Completadas por Usuario

**GET** `/respondidas/{email}`

Retorna todas las encuestas que un usuario ha completado (respondió todas las preguntas).

**Parámetros:**
| Nombre | Tipo   | Ubicación | Descripción                    |
|--------|--------|-----------|--------------------------------|
| email  | string | Path      | Correo electrónico del usuario |

**Respuestas:**

| Código | Descripción                    |
|--------|--------------------------------|
| 200    | Lista de encuestas completadas |

**Ejemplo de Respuesta (200):**
```json
[
  {
    "surveyId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
    "titulo": "Encuesta de Satisfacción del Evento: Concierto de Rock",
    "eventoId": "a1b2c3d4-5678-90ab-cdef-1234567890ab",
    "completedAt": "2024-01-15T18:50:00Z"
  }
]
```

---

## Escalas de Valoración

Todos los endpoints que manejan valores de respuestas usan la siguiente escala:

| Valor | Enum          | Significado      |
|-------|---------------|------------------|
| 1     | mediocre      | Muy insatisfecho |
| 2     | malo          | Insatisfecho     |
| 3     | regular       | Neutral          |
| 4     | excelente     | Satisfecho       |
| 5     | extraordinario| Muy satisfecho   |

---

## Códigos de Error Comunes

| Código | Descripción                                          |
|--------|------------------------------------------------------|
| 400    | Bad Request - Parámetros inválidos o faltantes      |
| 404    | Not Found - Recurso no encontrado                    |
| 409    | Conflict - Conflicto (ej: respuesta duplicada)       |
| 500    | Internal Server Error - Error interno del servidor   |

---

## Notas de Integración

### Flujo Típico de Usuario

1. **Listar encuestas pendientes:** `GET /pendientes/{email}`
2. **Obtener preguntas de una encuesta:** `GET /detailSurveyQuestion/{surveyId}`
3. **Por cada pregunta, enviar respuesta:** `POST /registerRespuesta`
4. **Verificar respuestas guardadas:** `GET /respuestasEventoUsuario/{eventId}/{email}`

### Validaciones del Backend

- Las encuestas solo se crean automáticamente para eventos en estado "Publicado"
- Un usuario solo puede ver encuestas de eventos que haya pagado
- Un usuario no puede responder la misma pregunta dos veces
- Las respuestas deben estar en el rango 1-5

### Dependencias de Microservicios

Este servicio requiere que estén operativos:
- **Microservicio de Usuarios** (puerto 7181) - Para validar emails
- **Microservicio de Pagos** (puerto 7183) - Para verificar elegibilidad
- **Microservicio de Eventos** (puerto 5000) - Para verificar estado de eventos
