using Moq;
using surveys_services.application.DTOs;
using surveys_services.application.Interfaces;
using surveys_services.application.Queries.Handlers;
using surveys_services.application.Queries.Queries;
using surveys_services.domain.Entities;
using surveys_services.domain.Interfaces;
using Xunit;

namespace surveys_services.tests.Application.Handlers
{
    public class GetCompletedSurveysByUserHandlerTests
    {
        private readonly Mock<ISurveysRepository> _surveysRepositoryMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly GetCompletedSurveysByUserHandler _handler;

        public GetCompletedSurveysByUserHandlerTests()
        {
            _surveysRepositoryMock = new Mock<ISurveysRepository>();
            _userServiceMock = new Mock<IUserService>();
            _handler = new GetCompletedSurveysByUserHandler(_surveysRepositoryMock.Object, _userServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnCompletedSurveys_WhenUserHasAnsweredSome()
        {
            var email = "user@test.com";
            var userId = Guid.NewGuid();
            var query = new GetCompletedSurveysByUserQuery(email);

            var surveys = new List<Survey>
            {
                new Survey(Guid.NewGuid(), "Encuesta 1"),
                new Survey(Guid.NewGuid(), "Encuesta 2")
            };

            _userServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(email))
                .ReturnsAsync(userId);

            _surveysRepositoryMock.Setup(r => r.GetAllSurveysAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(surveys);

            _surveysRepositoryMock.Setup(r => r.VerificarSiUsuarioRespondioAsync(surveys[0].Id, userId))
                .ReturnsAsync(true);

            _surveysRepositoryMock.Setup(r => r.VerificarSiUsuarioRespondioAsync(surveys[1].Id, userId))
                .ReturnsAsync(false);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Single(result);
            Assert.Equal(surveys[0].Id, result[0].SurveyId);
            Assert.Equal(surveys[0].Titulo, result[0].SurveyTitle);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenUserHasNotAnsweredAny()
        {
            var email = "newuser@test.com";
            var userId = Guid.NewGuid();
            var query = new GetCompletedSurveysByUserQuery(email);

            var surveys = new List<Survey> { new Survey(Guid.NewGuid(), "Encuesta 1") };

            _userServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(email))
                .ReturnsAsync(userId);

            _surveysRepositoryMock.Setup(r => r.GetAllSurveysAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(surveys);

            _surveysRepositoryMock.Setup(r => r.VerificarSiUsuarioRespondioAsync(It.IsAny<Guid>(), userId))
                .ReturnsAsync(false);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Empty(result);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenUserServiceFails()
        {
            var query = new GetCompletedSurveysByUserQuery("error@test.com");

            _userServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("External Service Error"));

            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        }
    }
}