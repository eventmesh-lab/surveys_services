using Microsoft.EntityFrameworkCore;
using surveys_services.domain.Entities;
using surveys_services.domain.Interfaces;
using surveys_services.infrastructure.Mappers;
using surveys_services.infrastructure.Persistence.Context;
using surveys_services.infrastructure.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;

namespace surveys_services.infrastructure.Persistence.Repositories
{
    public class AnswerRepositoryPostgres : IAnswerRepository
    {
        public readonly AppDbContext _context;

        public AnswerRepositoryPostgres(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddUAnswernPostgres(Answer answer, CancellationToken cancellationToken)
        {
            var model = AnswerMappers.ToPostgres(answer);
            _context.Answers.Add(model);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Answer?> ObtenerRespuestaPorUsuarioPreguntaEncuestaAsync(Guid encuestaId, Guid preguntaId, Guid usuarioId)
        {
            var answerPostgres = await (from answer in _context.Answers
                    join question in _context.Questions on answer.PreguntaId equals question.Id
                    where question.IdEncuesta == encuestaId
                          && question.Id == preguntaId        
                          && answer.UsuarioId == usuarioId    
                    select answer)
                .FirstOrDefaultAsync();

            if (answerPostgres == null)
            {
                return null;
            }
            return AnswerMappers.ToDomain(answerPostgres);
        }

        public async Task<List<Answer>> ObtenerRespuestasPorPreguntayEncuestaAsync(Guid encuestaId, Guid preguntaId)
        { 
            var answersPostgres = await (from answer in _context.Answers
                    join question in _context.Questions on answer.PreguntaId equals question.Id
                    where question.IdEncuesta == encuestaId
                          && question.Id == preguntaId
                    select answer)
                .ToListAsync();

            return answersPostgres
                .Select(a => AnswerMappers.ToDomain(a))
                .ToList();
        }

        public async Task<List<Answer>> ObtenerRespuestasPorEncuestaYUsuarioAsync(Guid encuestaId, Guid usuarioId)
        {
            
            var answersPostgres = await (from answer in _context.Answers
                    join question in _context.Questions on answer.PreguntaId equals question.Id
                    where question.IdEncuesta == encuestaId
                          && answer.UsuarioId == usuarioId
                    select answer)
                .AsNoTracking() 
                .ToListAsync();

            return answersPostgres
                .Select(a => AnswerMappers.ToDomain(a))
                .ToList();
        }
    }
}
