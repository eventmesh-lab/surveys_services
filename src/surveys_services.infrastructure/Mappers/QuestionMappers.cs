using surveys_services.domain.Entities;
using surveys_services.infrastructure.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace surveys_services.infrastructure.Mappers
{
    public class QuestionMappers
    {
        public static Question ToDomain(QuestionPostgres model)
        {
            return new Question(model.Id, model.IdEncuesta, model.Text);
        }

        public static QuestionPostgres ToPostgres(Question survey)
        {
            return new QuestionPostgres
            {
                Id = survey.Id,
                IdEncuesta = survey.IdEncuesta,
                Text = survey.Text
            };
        }
    }
}
