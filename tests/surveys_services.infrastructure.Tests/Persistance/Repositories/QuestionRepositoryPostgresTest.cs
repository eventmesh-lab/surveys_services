using Microsoft.EntityFrameworkCore;
using surveys_services.domain.Entities;
using surveys_services.infrastructure.Persistence.Context;
using surveys_services.infrastructure.Persistence.Models;
using surveys_services.infrastructure.Persistence.Repositories;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace surveys_services.tests.Infrastructure.Repositories
{
    public class QuestionRepositoryPostgresTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task CrearPreguntaAsync_ShouldSaveQuestionCorrectly()
        {
            var context = GetDbContext();
            var repository = new QuestionRepositoryPostgres(context);
            var idEncuesta = Guid.NewGuid();
            var question = new Question(idEncuesta, "¿Cómo calificaría la organización?");

            await repository.CrearPreguntaAsync(question, CancellationToken.None);

            var savedQuestion = await context.Questions.FirstOrDefaultAsync();
            Assert.NotNull(savedQuestion);
            Assert.Equal(question.Id, savedQuestion.Id);
            Assert.Equal(idEncuesta, savedQuestion.IdEncuesta);
            Assert.Equal("¿Cómo calificaría la organización?", savedQuestion.Text);
        }

        [Fact]
        public async Task ObtenerPreguntasPorEncuestaAsync_ShouldReturnQuestions_WhenSurveyExists()
        {
            var context = GetDbContext();
            var repository = new QuestionRepositoryPostgres(context);
            var idEncuesta = Guid.NewGuid();

            context.Questions.AddRange(new List<QuestionPostgres>
            {
                new QuestionPostgres { Id = Guid.NewGuid(), IdEncuesta = idEncuesta, Text = "Pregunta 1" },
                new QuestionPostgres { Id = Guid.NewGuid(), IdEncuesta = idEncuesta, Text = "Pregunta 2" },
                new QuestionPostgres { Id = Guid.NewGuid(), IdEncuesta = Guid.NewGuid(), Text = "Pregunta de otra encuesta" }
            });
            await context.SaveChangesAsync();

            var result = await repository.ObtenerPreguntasPorEncuestaAsync(idEncuesta);

            Assert.Equal(2, result.Count);
            Assert.All(result, q => Assert.Equal(idEncuesta, q.IdEncuesta));
        }

        [Fact]
        public async Task ObtenerPreguntasPorEncuestaAsync_ShouldReturnEmptyList_WhenSurveyHasNoQuestions()
        {
            var context = GetDbContext();
            var repository = new QuestionRepositoryPostgres(context);

            var result = await repository.ObtenerPreguntasPorEncuestaAsync(Guid.NewGuid());

            Assert.Empty(result);
        }

        [Fact]
        public async Task Constructor_ShouldThrowArgumentNullException_WhenContextIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new QuestionRepositoryPostgres(null));
        }
    }
}