using surveys_services.domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveys_services.infrastructure.Persistence.Models
{
    public class AnswerPostgres
    {
        public Guid Id { get; set; }
        public Guid PreguntaId { get; set; }
        public Guid UsuarioId { get; set; }
        public DateTime FechaRespuesta { get; set; }
        public string Valor { get; set; }
    }
}
