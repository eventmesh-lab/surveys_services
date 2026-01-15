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
        private readonly IUserService _userService;

        public GetPendingSurveysByUserHandler(
            IPagosService pagosService,
            IEventosService eventosService,
            ISurveysRepository surveysRepository,
            IQuestionRepository questionRepository,
            IUserService userService)
        {
            _pagosService = pagosService;
            _eventosService = eventosService;
            _surveysRepository = surveysRepository;
            _questionRepository = questionRepository;
            _userService = userService;
        }

        public async Task<List<PendingSurveyDto>> Handle(GetPendingSurveysByUserQuery request, CancellationToken cancellationToken)
        {
            var result = new List<PendingSurveyDto>();
            var UserId = await _userService.ObtenerUsuarioPorEmailAsync(request.Email);
            var eventosPagadosIds = await _pagosService.ObtenerEventosPagadosPorUsuarioAsync(request.Email, cancellationToken);

            if (eventosPagadosIds == null || !eventosPagadosIds.Any())
            {
                throw new ApplicationException("No tiene encuestas por responder");
            }

            foreach (var eventoId in eventosPagadosIds)
            {
                try
                {
                    if (eventoId == Guid.Empty) continue;

                    var estadoEvento = await _eventosService.ObtenerEstadoEventoAsync(eventoId, cancellationToken);

                    if (estadoEvento == null)
                    {
                        continue;
                    }

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

                        bool encuestaRespondida = await _surveysRepository.VerificarSiUsuarioRespondioAsync(survey.Id, UserId);

                        if (!encuestaRespondida)
                        {
                            if (result.Any(r => r.Id == survey.Id))
                            {
                                continue;
                            }

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
                catch (Exception)
                {
                    continue;
                }
            }
            if (result == null || !result.Any())
            {
                throw new KeyNotFoundException("No tiene encuestas pendientes por responder.");
            }

            return result;
        }
    }
}