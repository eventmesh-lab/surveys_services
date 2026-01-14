using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveys_services.application.DTOs
{
    public class RegisterAnswerDto
    {
        public Guid EncuestaId { get; set; } 
        public Guid PreguntaId { get; set; }
        public Guid UsuarioId { get; set; }
        public int Valor { get; set; }
    }
}
