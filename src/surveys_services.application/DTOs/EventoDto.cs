using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace surveys_services.application.DTOs
{
    public class EventoDto
    {
        [JsonPropertyName("Id")]
        public Guid Id { get; set; }
        [JsonPropertyName("Nombre")]
        public string Nombre { get; set; }
        [JsonPropertyName("Estado")]
        public string Estado { get; set; }
    }
}
