using Moq;
using surveys_services.application.DTOs;
using surveys_services.application.Interfaces;
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
    public class GetUserSurveyAnswersByEventHandlerTests
    {
        private readonly Mock<ISurveysRepository> _surveysRepositoryMock;
        private readonly Mock<IQuestionRepository> _questionRepositoryMock;
        private readonly Mock<IAnswerRepository> _answerRepositoryMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly GetUserSurveyAnswersByEventHandler _handler;

        public GetUserSurveyAnswersByEventHandlerTests()
        {
            _surveysRepositoryMock = new Mock<ISurveysRepository>();
            _questionRepositoryMock = new Mock<IQuestionRepository>();
            _answerRepositoryMock = new Mock<IAnswerRepository>();
            _userServiceMock = new Mock<IUserService>();

            _handler = new GetUserSurveyAnswersByEventHandler(
                _surveysRepositoryMock.Object,
                _questionRepositoryMock.Object,
                _answerRepositoryMock.Object,
                _userServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnDetailedResults_WhenSurveyAndAnswersExist()
        {
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var email = "test@user.com";
            var query = new GetUserSurveyAnswersByEventQuery(email, eventId);

            var survey = new Survey(eventId, "Encuesta de Satisfacción");
            var questions = new List<Question>
            {
                new Question(Guid.NewGuid(), survey.Id, "Pregunta 1")
            };
            var answers = new List<Answer>
            {
                new Answer(questions[0].Id, userId, EnumValue.excelente)
            };

            _surveysRepositoryMock.Setup(r => r.ObtenerEncuestaPorEventoAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(survey);
            _userServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(email))
                .ReturnsAsync(userId);
            _questionRepositoryMock.Setup(r => r.ObtenerPreguntasPorEncuestaAsync(survey.Id))
                .ReturnsAsync(questions);
            _answerRepositoryMock.Setup(r => r.ObtenerRespuestasPorEncuestaYUsuarioAsync(survey.Id, userId))
                .ReturnsAsync(answers);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(survey.Id, result.SurveyId);
            Assert.Single(result.Details);
            Assert.Equal(EnumValue.excelente.ToString(), result.Details[0].AnswerValue);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenSurveyNotFound()
        {
            var eventId = Guid.NewGuid();
            var email = "test@user.com";
            var query = new GetUserSurveyAnswersByEventQuery(email, eventId);

            _surveysRepositoryMock.Setup(r => r.ObtenerEncuestaPorEventoAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Survey)null);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task Handle_ShouldReturnSinResponder_WhenQuestionHasNoAnswer()
        {
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var email = "test@user.com";
            var query = new GetUserSurveyAnswersByEventQuery(email, eventId);

            var survey = new Survey(eventId, "Encuesta");
            var questions = new List<Question> { new Question(Guid.NewGuid(), survey.Id, "P1") };

            _surveysRepositoryMock.Setup(r => r.ObtenerEncuestaPorEventoAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(survey);
            _userServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(email))
                .ReturnsAsync(userId);
            _questionRepositoryMock.Setup(r => r.ObtenerPreguntasPorEncuestaAsync(survey.Id))
                .ReturnsAsync(questions);
            _answerRepositoryMock.Setup(r => r.ObtenerRespuestasPorEncuestaYUsuarioAsync(survey.Id, userId))
                .ReturnsAsync(new List<Answer>());

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal("Sin responder", result.Details[0].AnswerValue);
            Assert.Null(result.Details[0].AnswerDate);
        }
    }
}