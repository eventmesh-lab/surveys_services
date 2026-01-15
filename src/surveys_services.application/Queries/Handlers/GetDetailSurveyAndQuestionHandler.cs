using MediatR;
using surveys_services.application.DTOs;
using surveys_services.application.Queries.Queries;
using surveys_services.domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using surveys_services.domain.Constants;
using surveys_services.domain.Entities;

namespace surveys_services.application.Queries.Handlers
{
    public class GetDetailSurveyAndQuestionHandler : IRequestHandler<GetDetailSurveyAndQuestionQuery, SurveyAndQuestionDtoResponse>
    {
        private readonly ISurveysRepository _surveysRepository;
        private readonly IQuestionRepository _questionRepository;

        public GetDetailSurveyAndQuestionHandler(
            ISurveysRepository surveysRepository,
            IQuestionRepository questionRepository)
        {
            _surveysRepository = surveysRepository;
            _questionRepository = questionRepository;
        }

        public async Task<SurveyAndQuestionDtoResponse> Handle(GetDetailSurveyAndQuestionQuery request,
            CancellationToken cancellationToken)
        {
            var survey = await _surveysRepository.ObtenerEncuestaPorIdAsync(request.idSurvey, cancellationToken);

            if (survey == null)
            {
                return null;
            }

            var questions = await _questionRepository.ObtenerPreguntasPorEncuestaAsync(request.idSurvey);

            var result = new SurveyAndQuestionDtoResponse
            {
                idSurvey = survey.Id,
                Titulo = survey.Titulo,
                questions = questions.Select(q => new QuestionDtoResponse
                {
                    Id = q.Id,
                    question = q.Text
                }).ToList()
            };

            return result;
        }
    }
}
