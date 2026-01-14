using surveys_services.domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveys_services.domain.Interfaces
{
    public interface IAnswerRepository
    {
        Task AddUAnswernPostgres(Answer answer, CancellationToken cancellationToken);
        Task<Answer?> ObtenerRespuestaPorUsuarioPreguntaEncuestaAsync(Guid encuestaId, Guid preguntaId, Guid usuarioId);
        Task<List<Answer>> ObtenerRespuestasPorPreguntayEncuestaAsync(Guid encuestaId, Guid preguntaId);
        Task<List<Answer>> ObtenerRespuestasPorEncuestaYUsuarioAsync(Guid encuestaId, Guid usuarioId);
    }
}
