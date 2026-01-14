using surveys_services.domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveys_services.application.Interfaces
{
    public interface IEventosService
    {
        Task<Evento?> ObtenerEstadoEventoAsync(Guid eventoId, CancellationToken cancellationToken);
    }
}
