using surveys_services.domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveys_services.domain.Interfaces
{
    public interface ISurveysRepository
    {
        Task CrearEncuestaAsync(Survey encuesta, CancellationToken cancellationToken);
        Task<Survey?> ObtenerEncuestaPorIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Survey?> ObtenerEncuestaPorEventoAsync(Guid idEvento, CancellationToken cancellationToken);
        Task<List<Survey>> GetAllSurveysAsync(CancellationToken cancellationToken);
        Task<bool> VerificarSiUsuarioRespondioAsync(Guid surveyId, Guid userId);
    }
}
