using Moq;
using surveys_services.application.DTOs;
using surveys_services.application.Queries.Handlers;
using surveys_services.application.Queries.Queries;
using surveys_services.domain.Entities;
using surveys_services.domain.Enums;
using surveys_services.domain.Interfaces;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace surveys_services.tests.Application.Handlers
{
    public class PromedioEncuestaPorEventoHandlerTests
    {
        private readonly Mock<ISurveysRepository> _surveysRepositoryMock;
        private readonly Mock<IQuestionRepository> _questionRepositoryMock;
        private readonly Mock<IAnswerRepository> _answerRepositoryMock;
        private readonly PromedioEncuestaPorEventoHandler _handler;

        public PromedioEncuestaPorEventoHandlerTests()
        {
            _surveysRepositoryMock = new Mock<ISurveysRepository>();
            _questionRepositoryMock = new Mock<IQuestionRepository>();
            _answerRepositoryMock = new Mock<IAnswerRepository>();

            _handler = new PromedioEncuestaPorEventoHandler(
                _surveysRepositoryMock.Object,
                _questionRepositoryMock.Object,
                _answerRepositoryMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnCalculatedStats_WhenAnswersExist()
        {
            var eventId = Guid.NewGuid();
            var surveyId = Guid.NewGuid();
            var query = new PromedioEncuestaPorEventoQuery(eventId);
            var survey = new Survey(surveyId, eventId, "Encuesta de Satisfacción", DateTime.UtcNow);

            var questionId = Guid.NewGuid();
            var questions = new List<Question>
            {
                new Question(questionId, surveyId, "Pregunta Test")
            };

            var answers = new List<Answer>
            {
                new Answer(questionId, Guid.NewGuid(), EnumValue.malo),
                new Answer(questionId, Guid.NewGuid(), EnumValue.excelente)
            };

            _surveysRepositoryMock.Setup(r => r.ObtenerEncuestaPorEventoAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(survey);

            _questionRepositoryMock.Setup(r => r.ObtenerPreguntasPorEncuestaAsync(surveyId))
                .ReturnsAsync(questions);

            _answerRepositoryMock.Setup(r => r.ObtenerRespuestasPorPreguntayEncuestaAsync(surveyId, questionId))
                .ReturnsAsync(answers);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(eventId, result.EventoId);
            Assert.Single(result.QuestionsStats);
            Assert.Equal(2, result.QuestionsStats[0].CantidadRespuestas);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenSurveyNotFound()
        {
            var eventId = Guid.NewGuid();
            var query = new PromedioEncuestaPorEventoQuery(eventId);

            _surveysRepositoryMock.Setup(r => r.ObtenerEncuestaPorEventoAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Survey)null);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task Handle_ShouldReturnZeroAverage_WhenNoAnswersExistForQuestion()
        {
            var eventId = Guid.NewGuid();
            var surveyId = Guid.NewGuid();
            var query = new PromedioEncuestaPorEventoQuery(eventId);
            var survey = new Survey(surveyId, eventId, "Encuesta", DateTime.UtcNow);

            var questions = new List<Question>
            {
                new Question(Guid.NewGuid(), surveyId, "P1")
            };

            _surveysRepositoryMock.Setup(r => r.ObtenerEncuestaPorEventoAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(survey);

            _questionRepositoryMock.Setup(r => r.ObtenerPreguntasPorEncuestaAsync(surveyId))
                .ReturnsAsync(questions);

            _answerRepositoryMock.Setup(r => r.ObtenerRespuestasPorPreguntayEncuestaAsync(surveyId, It.IsAny<Guid>()))
                .ReturnsAsync(new List<Answer>());

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal(0, result.QuestionsStats[0].PromedioCalculado);
            Assert.Equal(0, result.QuestionsStats[0].CantidadRespuestas);
        }

        [Fact]
        public async Task Handle_ShouldReturnSurveyDetail_WhenSurveyExists()
        {
            var surveyId = Guid.NewGuid();
            var query = new GetDetailSurveyAndQuestionQuery(surveyId);
            var survey = new Survey(surveyId, Guid.NewGuid(), "Satisfacción del Cliente", DateTime.UtcNow);

            var questions = new List<Question>
            {
                new Question(Guid.NewGuid(), surveyId, "¿Cómo calificaría el servicio?"),
                new Question(Guid.NewGuid(), surveyId, "¿Lo recomendaría?")
            };

            _surveysRepositoryMock.Setup(r => r.ObtenerEncuestaPorIdAsync(surveyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(survey);

            _questionRepositoryMock.Setup(r => r.ObtenerPreguntasPorEncuestaAsync(surveyId))
                .ReturnsAsync(questions);

            var result = await (new GetDetailSurveyAndQuestionHandler(_surveysRepositoryMock.Object, _questionRepositoryMock.Object))
                .Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(surveyId, result.idSurvey);
        }
    }
}