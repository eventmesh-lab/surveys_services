using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using surveys_services.domain.Entities;
using surveys_services.domain.Interfaces;
using surveys_services.infrastructure.Mappers;
using surveys_services.infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace surveys_services.infrastructure.Persistence.Repositories
{
    public class SurveysRepositoryPostgres : ISurveysRepository
    {
        private readonly AppDbContext _context;
        public readonly IQuestionRepository _questionRepository;
        public readonly IAnswerRepository _answerRepository;

        public SurveysRepositoryPostgres(AppDbContext context, IQuestionRepository questionRepository, IAnswerRepository answerRepository)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); ;
            _questionRepository = questionRepository ?? throw new ArgumentNullException(nameof(questionRepository));
            _answerRepository = answerRepository ?? throw new ArgumentNullException(nameof(answerRepository));
        }

        public async Task CrearEncuestaAsync(Survey encuesta, CancellationToken cancellationToken)
        {
            var encuestaModel = SurveyMappers.ToPostgres(encuesta);
            await _context.Surveys.AddAsync(encuestaModel);
            await _context.SaveChangesAsync();
        }

        public async Task<Survey?> ObtenerEncuestaPorIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var surveysModel = await _context.Surveys
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            if (surveysModel == null)
            {
                return null;
            }
            return SurveyMappers.ToDomain(surveysModel);
        }

        public async Task<Survey?> ObtenerEncuestaPorEventoAsync(Guid idEvento, CancellationToken cancellationToken)
        {
            var surveysModel = await _context.Surveys
                .FirstOrDefaultAsync(u => u.EventoId == idEvento, cancellationToken);
            if (surveysModel == null)
            {
                return null;
            }
            return SurveyMappers.ToDomain(surveysModel);
        }

        public async Task<List<Survey>> GetAllSurveysAsync(CancellationToken cancellationToken)
        {
            var surveysModels = await _context.Surveys.ToListAsync(cancellationToken);

            var surveys = surveysModels
                .Select(SurveyMappers.ToDomain)
                .ToList();

            return  surveys;
        }

        public async Task<List<Survey>> ObtenerEncuestasPendientesAsync(Guid userId, List<Guid> eventosPagadosIds, CancellationToken cancellationToken)
        {
            var encuestasRespondidasIds = await _context.Answers
                .AsNoTracking() 
                .Where(answer => answer.UsuarioId == userId)
                .Join(_context.Questions,
                    answer => answer.PreguntaId,
                    question => question.Id,
                    (answer, question) => question.IdEncuesta)
                .Distinct() 
                .ToListAsync(cancellationToken);

            var surveysModels = await _context.Surveys
                .AsNoTracking()
                .Where(s => eventosPagadosIds.Contains(s.EventoId)
                            && !encuestasRespondidasIds.Contains(s.Id))
                .ToListAsync(cancellationToken);

            return surveysModels
                .Select(SurveyMappers.ToDomain)
                .ToList();
        }

        public async Task<bool> VerificarSiUsuarioRespondioAsync(Guid surveyId, Guid userId)
        {
            var preguntas = await _questionRepository.ObtenerPreguntasPorEncuestaAsync(surveyId);

            if (preguntas == null || !preguntas.Any())
            {
                return false;
            }

            foreach (var pregunta in preguntas)
            {
                var respuesta = await _answerRepository.ObtenerRespuestaPorUsuarioPreguntaEncuestaAsync(surveyId, pregunta.Id, userId);

                if (respuesta != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

