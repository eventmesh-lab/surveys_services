using surveys_services.domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveys_services.domain.Entities
{
    public class Answer
    {
        
        public Guid Id { get;  set; }
        public Guid PreguntaId { get; set; }
        public Guid UsuarioId { get;  set; } 
        public DateTime FechaRespuesta { get;  set; }
        public EnumValue Valor { get;  set; }

        public Answer(Guid preguntaId, Guid usuarioId, EnumValue valor)
        {
            Id = Guid.NewGuid();
            PreguntaId = preguntaId;
            UsuarioId = usuarioId;
            Valor = valor;
            FechaRespuesta = DateTime.UtcNow;
        }
        public Answer(Guid id, Guid preguntaId, Guid usuarioId, EnumValue valor)
        {
            Id = id;
            PreguntaId = preguntaId;
            UsuarioId = usuarioId;
            Valor = valor;
            FechaRespuesta = DateTime.UtcNow;
        }
    }
}