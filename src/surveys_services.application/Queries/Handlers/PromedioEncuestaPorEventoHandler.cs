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
    
    public class PromedioEncuestaPorEventoHandler : IRequestHandler<PromedioEncuestaPorEventoQuery, PromedioEventSurveyDto>
    {
        private readonly ISurveysRepository _surveysRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IAnswerRepository _answerRepository;

        public PromedioEncuestaPorEventoHandler(
            ISurveysRepository surveysRepository,
            IQuestionRepository questionRepository,
            IAnswerRepository answerRepository)
        {
            _surveysRepository = surveysRepository;
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
        }

        public async Task<PromedioEventSurveyDto> Handle(PromedioEncuestaPorEventoQuery request, CancellationToken cancellationToken)
        {
            var survey = await _surveysRepository.ObtenerEncuestaPorEventoAsync(request.EventoId, cancellationToken);

            if (survey == null)
            {
                return null;
            }

            var statsDto = new PromedioEventSurveyDto
            {
                EventoId = request.EventoId,
                SurveyId = survey.Id,
                SurveyTitle = survey.Titulo
            };

            var questions = await _questionRepository.ObtenerPreguntasPorEncuestaAsync(survey.Id);

            foreach (var question in questions)
            {
                var answers = await _answerRepository.ObtenerRespuestasPorPreguntayEncuestaAsync(survey.Id, question.Id);

                double promedio = 0;
                int totalRespuestas = 0;

                if (answers != null && answers.Any())
                {
                    totalRespuestas = answers.Count;
                    Console.WriteLine("Promedio");
                    promedio = answers.Average(a => (int)a.Valor);
                }

                statsDto.QuestionsStats.Add(new PromedioQuestionDto
                {
                    QuestionId = question.Id,
                    QuestionText = question.Text,
                    PromedioCalculado = Math.Round(promedio, 2), 
                    CantidadRespuestas = totalRespuestas
                });
            }

            return statsDto;
        }
    }
}
