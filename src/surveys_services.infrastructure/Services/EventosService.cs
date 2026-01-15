using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using surveys_services.application.DTOs;
using surveys_services.application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using surveys_services.domain.Entities;

namespace surveys_services.infrastructure.Services
{
    public class EventosService : IEventosService
    {
        private readonly HttpClient _httpClient;

        public EventosService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        private static readonly List<EventoDto> _eventosMock = new()
        {
            new EventoDto
            {
                Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa7"),
                Nombre = "Concierto de Rock",
                Estado = "Finalizado" // Este debería permitir ver la encuesta
            },
            new EventoDto
            {
                Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                Nombre = "Taller de Cocina",
                Estado = "Finalizado"
            }
        };

        public async Task<Evento?> ObtenerEstadoEventoAsync(Guid eventoId, CancellationToken cancellationToken)
        {
            try
            {
                /* var response = await _httpClient.GetAsync($"http://localhost:5002/api/Eventos/{eventoId}");

                 if (!response.IsSuccessStatusCode)
                 {
                     return null;
                 }

                 var contenido = await response.Content.ReadAsStringAsync();
                 Console.WriteLine(contenido);

                 var dto = JsonSerializer.Deserialize<EventoDto>(contenido, new JsonSerializerOptions
                 {
                     PropertyNameCaseInsensitive = true
                 });*/

                 
                var dto = _eventosMock.FirstOrDefault(e => e.Id == eventoId);
                if (dto == null)
                {
                    return null;
                }
                var evento = new Evento(
                    dto.Id,
                    dto.Nombre,
                    dto.Estado
                );

                return evento;
            }
            catch (Exception)
            {
               throw new ArgumentException("Ocurrio un error al obtener el evento");
            }
        }
    }
}
