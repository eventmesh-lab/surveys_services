using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveys_services.domain.Entities
{
    public class Evento
    {
        public Guid Id { get;  set; }
        public string Nombre { get;  set; }
        public string Estado { get; set; }

        public Evento (Guid id, string nombre, string estado)
        {
            Id = id;
            Nombre = nombre;
            Estado = estado;
        }
    }
}
