using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using surveys_services.application.DTOs;
using surveys_services.application.Interfaces;

namespace surveys_services.infrastructure.Services
{
    public class PagosService : IPagosService
    {
        private readonly HttpClient _httpClient;

        public PagosService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<List<Guid>> ObtenerEventosPagadosPorUsuarioAsync(Guid correo, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"http://localhost:7183/api/payments/obtenertHistorialPagosUsuario/{correo}",
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var contenido = await response.Content.ReadAsStringAsync(cancellationToken);
                Console.WriteLine(contenido);

                // Tu API devuelve una lista de HistorialPagosDTO, no un solo objeto
                var dtoList = JsonSerializer.Deserialize<List<HistorialPagoExternalDto>>(contenido, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (dtoList == null)
                {
                    return null;
                }

                var eventosIds = dtoList
                    .Where(p => p.IdEvento != Guid.Empty)
                    .Select(p => p.IdEvento)
                    .Distinct()
                    .ToList();

                return eventosIds;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }

        }
    }
}
