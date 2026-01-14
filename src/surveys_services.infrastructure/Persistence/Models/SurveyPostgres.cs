using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveys_services.infrastructure.Persistence.Models
{
    public class SurveyPostgres
    {
        public Guid Id { get;  set; }
        public Guid EventoId { get;  set; }
        public string Titulo { get;  set; }
        public string FechaCreacion { get;  set; }

    }
}
