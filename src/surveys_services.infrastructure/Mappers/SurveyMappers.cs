using surveys_services.domain.Entities;
using surveys_services.infrastructure.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;

namespace surveys_services.infrastructure.Mappers
{
    public class SurveyMappers
    {

        public static Survey ToDomain(SurveyPostgres model)
        {
            return new Survey(model.Id, model.EventoId, model.Titulo, DateTime.Parse(model.FechaCreacion));
        }

        public static SurveyPostgres ToPostgres(Survey survey)
        {
            return new SurveyPostgres
            {
                Id = survey.Id,
                EventoId = survey.EventoId,
                Titulo = survey.Titulo,
                FechaCreacion = survey.FechaCreacion.ToString()
            };
        }
    }
}
