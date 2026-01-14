using surveys_services.domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveys_services.domain.Interfaces
{
    public interface IQuestionRepository
    {
        Task CrearPreguntaAsync(Question question, CancellationToken cancellationToken);
        Task<List<Question>> ObtenerPreguntasPorEncuestaAsync(Guid idEncuesta);
    }
}
