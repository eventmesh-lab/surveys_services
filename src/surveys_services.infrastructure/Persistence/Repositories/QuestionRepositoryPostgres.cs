using Microsoft.EntityFrameworkCore;
using surveys_services.domain.Entities;
using surveys_services.infrastructure.Mappers;
using surveys_services.infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using surveys_services.domain.Interfaces;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;

namespace surveys_services.infrastructure.Persistence.Repositories
{
    public class QuestionRepositoryPostgres : IQuestionRepository
    {
        private readonly AppDbContext _context;

        public QuestionRepositoryPostgres(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); ;
        }

        public async Task CrearPreguntaAsync(Question question, CancellationToken cancellationToken)
        {
            var questionModel = QuestionMappers.ToPostgres(question);
            await _context.Questions.AddAsync(questionModel);
            await _context.SaveChangesAsync();
        }


        public async Task<List<Question>> ObtenerPreguntasPorEncuestaAsync(Guid idEncuesta)
        {
            var questionsPostgres = await _context.Questions
                .Where(q => q.IdEncuesta == idEncuesta)
                .ToListAsync();

            return questionsPostgres
                .Select(q => QuestionMappers.ToDomain(q))
                .ToList();
        }

    }
}
