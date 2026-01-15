using surveys_services.domain.Entities;
using surveys_services.domain.Enums;
using surveys_services.infrastructure.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveys_services.infrastructure.Mappers
{
    public class AnswerMappers
    {
        public static Answer ToDomain(AnswerPostgres answerPostgres)
        {
            EnumValue valorEnum;
            if (!Enum.TryParse(answerPostgres.Valor, out valorEnum))
            {
                throw new InvalidOperationException(
                    $"Valor '{answerPostgres.Valor}' no es válido para EnumValue");
            }

            return new Answer(
                answerPostgres.Id,
                answerPostgres.PreguntaId,
                answerPostgres.UsuarioId,
                valorEnum
            );

        }

        public static AnswerPostgres ToPostgres(Answer answer)
        {
            return new AnswerPostgres
            {
                Id = answer.Id,
                PreguntaId = answer.PreguntaId,
                UsuarioId = answer.UsuarioId,
                FechaRespuesta = answer.FechaRespuesta,
                Valor = answer.Valor.ToString() 
            };

        }
    }
}
