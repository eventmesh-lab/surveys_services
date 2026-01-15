using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveys_services.domain.Entities
{
    public class Survey
    {
        public Guid Id { get;  set; }
        public Guid EventoId { get;  set; } 
        public string Titulo { get;  set; }
        public DateTime FechaCreacion { get;  set; }

        public Survey(Guid eventoId, string titulo )
        {
            Id = Guid.NewGuid();
            EventoId = eventoId;
            Titulo = titulo;
            FechaCreacion = DateTime.UtcNow;
        }
        public Survey(Guid id, Guid eventoId, string titulo,  DateTime fecha)
        {
            Id = id;
            EventoId = eventoId;
            Titulo = titulo;
            FechaCreacion = fecha;
        }

    }
}
