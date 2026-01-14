using MediatR;
using surveys_services.application.DTOs;
using surveys_services.application.Interfaces;
using surveys_services.application.Queries.Queries;
using surveys_services.domain.Entities;
using surveys_services.domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using surveys_services.domain.Constants;

namespace surveys_services.application.Queries.Handlers
{
    public class GetPendingSurveysByUserHandler : IRequestHandler<GetPendingSurveysByUserQuery, List<PendingSurveyDto>>
    {
        private readonly IPagosService _pagosService;
        private readonly IEventosService _eventosService;
        private readonly ISurveysRepository _surveysRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IAnswerRepository _answerRepository;

        public GetPendingSurveysByUserHandler(
            IPagosService pagosService,
            IEventosService eventosService,
            ISurveysRepository surveysRepository,
            IQuestionRepository questionRepository,
            IAnswerRepository answerRepository)
        {
            _pagosService = pagosService;
            _eventosService = eventosService;
            _surveysRepository = surveysRepository;
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
        }

        public async Task<List<PendingSurveyDto>> Handle(GetPendingSurveysByUserQuery request, CancellationToken cancellationToken)
        {
            var result = new List<PendingSurveyDto>();

            var eventosPagadosIds = await _pagosService.ObtenerEventosPagadosPorUsuarioAsync(request.UserId, cancellationToken);

            if (eventosPagadosIds == null || !eventosPagadosIds.Any())
            {
                throw new ApplicationException("No tiene encuestas por responder");
            }

            foreach (var eventoId in eventosPagadosIds)
            {
                var estadoEvento = await _eventosService.ObtenerEstadoEventoAsync(eventoId, cancellationToken);

                
                if (string.Equals(estadoEvento.Estado, "Finalizado", StringComparison.OrdinalIgnoreCase))
                {
                 
                    var survey = await _surveysRepository.ObtenerEncuestaPorEventoAsync(eventoId, cancellationToken);

                    if (survey == null)
                    {
                        survey = new Survey(eventoId, $"Encuesta de Satisfacción del Evento: {estadoEvento.Nombre}");
                        await _surveysRepository.CrearEncuestaAsync(survey, cancellationToken);
                        foreach (var textoPregunta in SurveyConstants.DefaultQuestions)
                        {
                            var pregunta = new Question(survey.Id, textoPregunta);
                            await _questionRepository.CrearPreguntaAsync(pregunta, cancellationToken);
                        }
                    }

                    bool encuestaRespondida = await _surveysRepository.VerificarSiUsuarioRespondioAsync(survey.Id, request.UserId);

                  
                    if (!encuestaRespondida)
                    {
                        var surveyDto = new PendingSurveyDto
                        {
                            Id = survey.Id,
                            EventoId = survey.EventoId,
                            Titulo = survey.Titulo,
                            FechaCreacion = survey.FechaCreacion,
                        };

                        result.Add(surveyDto);
                    }
                }
            }

            if (result == null)
            {
                throw new ("No tiene encuestas por responder");
            }

            return result;
        }

        
    }
}
