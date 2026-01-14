using MediatR;
using surveys_services.application.DTOs;
using surveys_services.application.Queries.Queries;
using surveys_services.domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveys_services.application.Queries.Handlers
{
    public class GetUserSurveyAnswersByEventHandler : IRequestHandler<GetUserSurveyAnswersByEventQuery, SurveyResultByEventDto>
    {
        private readonly ISurveysRepository _surveysRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IAnswerRepository _answerRepository;

        public GetUserSurveyAnswersByEventHandler(
            ISurveysRepository surveysRepository,
            IQuestionRepository questionRepository,
            IAnswerRepository answerRepository)
        {
            _surveysRepository = surveysRepository;
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
        }

        public async Task<SurveyResultByEventDto> Handle(GetUserSurveyAnswersByEventQuery request, CancellationToken cancellationToken)
        {
            var survey = await _surveysRepository.ObtenerEncuestaPorEventoAsync(request.EventId, cancellationToken);

            if (survey == null)
            {
                return null;
            }

            var questions = await _questionRepository.ObtenerPreguntasPorEncuestaAsync(survey.Id);

            var userAnswers = await _answerRepository.ObtenerRespuestasPorEncuestaYUsuarioAsync(survey.Id, request.UserId);

            var resultDto = new SurveyResultByEventDto
            {
                SurveyId = survey.Id,
                SurveyTitle = survey.Titulo,
                EventId = request.EventId
            };

            foreach (var question in questions)
            {
                var matchingAnswer = userAnswers.FirstOrDefault(a => a.PreguntaId == question.Id);

                var detail = new QuestionAnswerDetailDto
                {
                    QuestionId = question.Id,
                    QuestionText = question.Text
                };

                if (matchingAnswer != null)
                {
                    detail.AnswerValue = matchingAnswer.Valor.ToString(); 
                    detail.AnswerDate = matchingAnswer.FechaRespuesta;
                }
                else
                {
                    detail.AnswerValue = "Sin responder";
                    detail.AnswerDate = null;
                }

                resultDto.Details.Add(detail);
            }

            return resultDto;
        }
    }
}
