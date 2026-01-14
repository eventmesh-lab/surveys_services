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
    public class GetCompletedSurveysByUserHandler : IRequestHandler<GetCompletedSurveysByUserQuery, List<CompletedSurveyDto>>
    {
        private readonly ISurveysRepository _surveysRepository;

        public GetCompletedSurveysByUserHandler(ISurveysRepository surveysRepository)
        {
            _surveysRepository = surveysRepository;
        }

        public async Task<List<CompletedSurveyDto>> Handle(GetCompletedSurveysByUserQuery request, CancellationToken cancellationToken)
        {
            var result = new List<CompletedSurveyDto>();

            var allSurveys = await _surveysRepository.GetAllSurveysAsync(cancellationToken);
            foreach (var survey in allSurveys)
            {
                bool yaRespondio = await _surveysRepository.VerificarSiUsuarioRespondioAsync(survey.Id, request.UserId);

                if (yaRespondio)
                {
                    result.Add(new CompletedSurveyDto
                    {
                        SurveyId = survey.Id,
                        EventId = survey.EventoId,
                        SurveyTitle = survey.Titulo
                    });
                }
            }

            return result;
        }
    }
}
