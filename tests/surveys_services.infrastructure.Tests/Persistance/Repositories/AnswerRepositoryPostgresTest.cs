using Microsoft.EntityFrameworkCore;
using surveys_services.domain.Entities;
using surveys_services.domain.Enums;
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
    public class AnswerRepositoryPostgresTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddUAnswernPostgres_ShouldSaveAnswerCorrectly()
        {
            var context = GetDbContext();
            var repository = new AnswerRepositoryPostgres(context);
            var answer = new Answer(Guid.NewGuid(), Guid.NewGuid(), EnumValue.excelente);

            await repository.AddUAnswernPostgres(answer, CancellationToken.None);

            var savedAnswer = await context.Answers.FirstOrDefaultAsync();
            Assert.NotNull(savedAnswer);
            Assert.Equal(answer.Id, savedAnswer.Id);
        }

        [Fact]
        public async Task ObtenerRespuestaPorUsuarioPreguntaEncuestaAsync_ShouldReturnAnswer_WhenExists()
        {
            var context = GetDbContext();
            var repository = new AnswerRepositoryPostgres(context);

            var encuestaId = Guid.NewGuid();
            var preguntaId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();

            context.Questions.Add(new QuestionPostgres { Id = preguntaId, IdEncuesta = encuestaId, Text = "Test Question" });
            context.Answers.Add(new AnswerPostgres { Id = Guid.NewGuid(), PreguntaId = preguntaId, UsuarioId = usuarioId, Valor = "5" });
            await context.SaveChangesAsync();

            var result = await repository.ObtenerRespuestaPorUsuarioPreguntaEncuestaAsync(encuestaId, preguntaId, usuarioId);

            Assert.NotNull(result);
            Assert.Equal(usuarioId, result.UsuarioId);
        }

        [Fact]
        public async Task ObtenerRespuestaPorUsuarioPreguntaEncuestaAsync_ShouldReturnNull_WhenNotExists()
        {
            var context = GetDbContext();
            var repository = new AnswerRepositoryPostgres(context);

            var result = await repository.ObtenerRespuestaPorUsuarioPreguntaEncuestaAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task ObtenerRespuestasPorPreguntayEncuestaAsync_ShouldReturnList()
        {
            var context = GetDbContext();
            var repository = new AnswerRepositoryPostgres(context);

            var encuestaId = Guid.NewGuid();
            var preguntaId = Guid.NewGuid();

            context.Questions.Add(new QuestionPostgres { Id = preguntaId, IdEncuesta = encuestaId, Text = "Test Question" });
            context.Answers.AddRange(new List<AnswerPostgres>
            {
                new AnswerPostgres { Id = Guid.NewGuid(), PreguntaId = preguntaId, UsuarioId = Guid.NewGuid(), Valor = "4" },
                new AnswerPostgres { Id = Guid.NewGuid(), PreguntaId = preguntaId, UsuarioId = Guid.NewGuid(), Valor = "2" }
            });
            await context.SaveChangesAsync();

            var result = await repository.ObtenerRespuestasPorPreguntayEncuestaAsync(encuestaId, preguntaId);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task ObtenerRespuestasPorEncuestaYUsuarioAsync_ShouldReturnFilteredList()
        {
            var context = GetDbContext();
            var repository = new AnswerRepositoryPostgres(context);

            var encuestaId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var pregunta1Id = Guid.NewGuid();

            context.Questions.Add(new QuestionPostgres { Id = pregunta1Id, IdEncuesta = encuestaId, Text = "Q1" });

            context.Answers.Add(new AnswerPostgres
            {
                Id = Guid.NewGuid(),
                PreguntaId = pregunta1Id,
                UsuarioId = usuarioId,
                Valor = "5"
            });
            await context.SaveChangesAsync();

            var result = await repository.ObtenerRespuestasPorEncuestaYUsuarioAsync(encuestaId, usuarioId);

            Assert.Single(result);
            Assert.Equal(usuarioId, result[0].UsuarioId);
        }
    }
}