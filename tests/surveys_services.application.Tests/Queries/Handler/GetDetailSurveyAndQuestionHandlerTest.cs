using Moq;
using surveys_services.application.DTOs;
using surveys_services.application.Queries.Handlers;
using surveys_services.application.Queries.Queries;
using surveys_services.domain.Entities;
using surveys_services.domain.Interfaces;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace surveys_services.tests.Application.Handlers
{
    public class GetDetailSurveyAndQuestionHandlerTests
    {
        private readonly Mock<ISurveysRepository> _surveysRepositoryMock;
        private readonly Mock<IQuestionRepository> _questionRepositoryMock;
        private readonly GetDetailSurveyAndQuestionHandler _handler;

        public GetDetailSurveyAndQuestionHandlerTests()
        {
            _surveysRepositoryMock = new Mock<ISurveysRepository>();
            _questionRepositoryMock = new Mock<IQuestionRepository>();
            _handler = new GetDetailSurveyAndQuestionHandler(
                _surveysRepositoryMock.Object,
                _questionRepositoryMock.Object);
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

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(surveyId, result.idSurvey);
            Assert.Equal("Satisfacción del Cliente", result.Titulo);
            Assert.Equal(2, result.questions.Count);
            Assert.Equal(questions[0].Text, result.questions[0].question);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenSurveyDoesNotExist()
        {
            var surveyId = Guid.NewGuid();
            var query = new GetDetailSurveyAndQuestionQuery(surveyId);

            _surveysRepositoryMock.Setup(r => r.ObtenerEncuestaPorIdAsync(surveyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Survey)null);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Null(result);
            _questionRepositoryMock.Verify(r => r.ObtenerPreguntasPorEncuestaAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyQuestions_WhenSurveyHasNoQuestions()
        {
            var surveyId = Guid.NewGuid();
            var query = new GetDetailSurveyAndQuestionQuery(surveyId);
            var survey = new Survey(surveyId, "Encuesta Vacía");

            _surveysRepositoryMock.Setup(r => r.ObtenerEncuestaPorIdAsync(surveyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(survey);

            _questionRepositoryMock.Setup(r => r.ObtenerPreguntasPorEncuestaAsync(surveyId))
                .ReturnsAsync(new List<Question>());

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result.questions);
        }
    }
}