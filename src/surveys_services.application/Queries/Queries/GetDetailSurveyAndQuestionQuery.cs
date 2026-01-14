using MediatR;
using surveys_services.application.DTOs;
using surveys_services.domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveys_services.application.Queries.Queries
{
    public class GetDetailSurveyAndQuestionQuery : IRequest<SurveyAndQuestionDtoResponse>
    {
        public Guid idSurvey { get; set; }
        public GetDetailSurveyAndQuestionQuery(Guid surveyId)
        {
            idSurvey = surveyId;
        }
    }
}
