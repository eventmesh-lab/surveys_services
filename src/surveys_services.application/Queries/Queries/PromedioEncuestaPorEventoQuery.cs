using MediatR;
using surveys_services.application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveys_services.application.Queries.Queries
{
    public class PromedioEncuestaPorEventoQuery : IRequest<PromedioEventSurveyDto>
    {
        public Guid EventoId { get; set; }

        public PromedioEncuestaPorEventoQuery(Guid eventoId)
        {
            EventoId = eventoId;
        }
    }
}
