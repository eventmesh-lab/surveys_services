using Microsoft.EntityFrameworkCore;
using Moq;
using surveys_services.domain.Entities;
using surveys_services.domain.Interfaces;
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
    public class SurveysRepositoryPostgresTests
    {
        private readonly Mock<IQuestionRepository> _questionRepositoryMock;
        private readonly Mock<IAnswerRepository> _answerRepositoryMock;

        public SurveysRepositoryPostgresTests()
        {
            _questionRepositoryMock = new Mock<IQuestionRepository>();
            _answerRepositoryMock = new Mock<IAnswerRepository>();
        }

        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task CrearEncuestaAsync_ShouldSaveSurveyCorrectly()
        {
            var context = GetDbContext();
            var repository = new SurveysRepositoryPostgres(context, _questionRepositoryMock.Object, _answerRepositoryMock.Object);
            var survey = new Survey(Guid.NewGuid(), "Encuesta Evento A");

            await repository.CrearEncuestaAsync(survey, CancellationToken.None);

            var savedSurvey = await context.Surveys.FirstOrDefaultAsync();
            Assert.NotNull(savedSurvey);
            Assert.Equal(survey.Id, savedSurvey.Id);
            Assert.Equal(survey.Titulo, savedSurvey.Titulo);
        }

        [Fact]
        public async Task ObtenerEncuestaPorIdAsync_ShouldReturnSurvey_WhenExists()
        {
            var context = GetDbContext();
            var repository = new SurveysRepositoryPostgres(context, _questionRepositoryMock.Object, _answerRepositoryMock.Object);
            var surveyId = Guid.NewGuid();
            context.Surveys.Add(new SurveyPostgres { Id = surveyId, Titulo = "Test Survey", EventoId = Guid.NewGuid(), FechaCreacion = DateTime.UtcNow.ToString() });
            await context.SaveChangesAsync();

            var result = await repository.ObtenerEncuestaPorIdAsync(surveyId, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(surveyId, result.Id);
        }

        [Fact]
        public async Task ObtenerEncuestaPorEventoAsync_ShouldReturnSurvey_WhenEventoMatches()
        {
            var context = GetDbContext();
            var repository = new SurveysRepositoryPostgres(context, _questionRepositoryMock.Object, _answerRepositoryMock.Object);
            var eventoId = Guid.NewGuid();
            context.Surveys.Add(new SurveyPostgres { Id = Guid.NewGuid(), Titulo = "Evento Survey", EventoId = eventoId, FechaCreacion = DateTime.UtcNow.ToString() });
            await context.SaveChangesAsync();

            var result = await repository.ObtenerEncuestaPorEventoAsync(eventoId, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(eventoId, result.EventoId);
        }

        [Fact]
        public async Task GetAllSurveysAsync_ShouldReturnAllRecords()
        {
            var context = GetDbContext();
            var repository = new SurveysRepositoryPostgres(context, _questionRepositoryMock.Object, _answerRepositoryMock.Object);
            context.Surveys.AddRange(new List<SurveyPostgres>
            {
                new SurveyPostgres { Id = Guid.NewGuid(), Titulo = "S1", EventoId = Guid.NewGuid(), FechaCreacion = DateTime.UtcNow.ToString()},
                new SurveyPostgres { Id = Guid.NewGuid(), Titulo = "S2", EventoId = Guid.NewGuid(), FechaCreacion = DateTime.UtcNow.ToString() }
            });
            await context.SaveChangesAsync();

            var result = await repository.GetAllSurveysAsync(CancellationToken.None);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task VerificarSiUsuarioRespondioAsync_ShouldReturnTrue_WhenAtLeastOneAnswerExists()
        {
            var context = GetDbContext();
            var repository = new SurveysRepositoryPostgres(context, _questionRepositoryMock.Object, _answerRepositoryMock.Object);
            var surveyId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var questions = new List<Question> { new Question(Guid.NewGuid(), surveyId, "Q1") };

            _questionRepositoryMock.Setup(r => r.ObtenerPreguntasPorEncuestaAsync(surveyId))
                .ReturnsAsync(questions);

            _answerRepositoryMock.Setup(r => r.ObtenerRespuestaPorUsuarioPreguntaEncuestaAsync(surveyId, questions[0].Id, userId))
                .ReturnsAsync(new Answer(questions[0].Id, userId, domain.Enums.EnumValue.extraordinario));

            var result = await repository.VerificarSiUsuarioRespondioAsync(surveyId, userId);

            Assert.True(result);
        }

        [Fact]
        public async Task VerificarSiUsuarioRespondioAsync_ShouldReturnFalse_WhenNoQuestionsOrAnswers()
        {
            var context = GetDbContext();
            var repository = new SurveysRepositoryPostgres(context, _questionRepositoryMock.Object, _answerRepositoryMock.Object);
            var surveyId = Guid.NewGuid();

            _questionRepositoryMock.Setup(r => r.ObtenerPreguntasPorEncuestaAsync(surveyId))
                .ReturnsAsync(new List<Question>());

            var result = await repository.VerificarSiUsuarioRespondioAsync(surveyId, Guid.NewGuid());

            Assert.False(result);
        }

        [Fact]
        public async Task ObtenerEncuestasPendientesAsync_ShouldFilterCorrectly()
        {
            var context = GetDbContext();
            var repository = new SurveysRepositoryPostgres(context, _questionRepositoryMock.Object, _answerRepositoryMock.Object);

            var userId = Guid.NewGuid();
            var eventId1 = Guid.NewGuid();
            var eventId2 = Guid.NewGuid();
            var surveyId1 = Guid.NewGuid();
            var surveyId2 = Guid.NewGuid();
            var questionId = Guid.NewGuid();

            context.Surveys.AddRange(new List<SurveyPostgres>
            {
                new SurveyPostgres { Id = surveyId1, EventoId = eventId1, Titulo = "Pendiente", FechaCreacion = DateTime.UtcNow.ToString() },
                new SurveyPostgres { Id = surveyId2, EventoId = eventId2, Titulo = "Respondida", FechaCreacion = DateTime.UtcNow.ToString() }
            });

            context.Questions.Add(new QuestionPostgres { Id = questionId, IdEncuesta = surveyId2, Text = "Q" });
            context.Answers.Add(new AnswerPostgres { Id = Guid.NewGuid(), PreguntaId = questionId, UsuarioId = userId, Valor = "5" });

            await context.SaveChangesAsync();

            var paidEvents = new List<Guid> { eventId1, eventId2 };

            var result = await repository.ObtenerEncuestasPendientesAsync(userId, paidEvents, CancellationToken.None);

            Assert.Single(result);
            Assert.Equal(surveyId1, result[0].Id);
        }
    }
}