using Moq;
using surveys_services.application.Commands.Commands;
using surveys_services.application.Commands.Handlers;
using surveys_services.application.DTOs;
using surveys_services.application.Interfaces;
using surveys_services.domain.Entities;
using surveys_services.domain.Enums;
using surveys_services.domain.Interfaces;
using Xunit;

namespace surveys_services.tests.Application.Handlers
{
    public class RegisterAnswerHandlerTests
    {
        private readonly Mock<IAnswerRepository> _answerRepositoryMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly RegisterAnswerHandler _handler;

        public RegisterAnswerHandlerTests()
        {
            _answerRepositoryMock = new Mock<IAnswerRepository>();
            _userServiceMock = new Mock<IUserService>();
            _handler = new RegisterAnswerHandler(_answerRepositoryMock.Object, _userServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnGuid_WhenRequestIsValid()
        {
            var userId = Guid.NewGuid();
            var dto = new RegisterAnswerDto
            {
                EncuestaId = Guid.NewGuid(),
                PreguntaId = Guid.NewGuid(),
                email = "user@test.com",
                Valor = 3
            };
            var command = new RegisterAnswerCommand(dto);

            _userServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(dto.email))
                .ReturnsAsync(userId);

            _answerRepositoryMock.Setup(r => r.ObtenerRespuestaPorUsuarioPreguntaEncuestaAsync(dto.EncuestaId, dto.PreguntaId, userId))
                .ReturnsAsync((Answer)null);

            _answerRepositoryMock.Setup(r => r.AddUAnswernPostgres(It.IsAny<Answer>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotEqual(Guid.Empty, result);
            _answerRepositoryMock.Verify(r => r.AddUAnswernPostgres(It.IsAny<Answer>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenValueIsInvalid()
        {
            var dto = new RegisterAnswerDto { Valor = 99 };
            var command = new RegisterAnswerCommand(dto);

            await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidOperationException_WhenAnswerAlreadyExists()
        {
            var userId = Guid.NewGuid();
            var dto = new RegisterAnswerDto
            {
                EncuestaId = Guid.NewGuid(),
                PreguntaId = Guid.NewGuid(),
                email = "user@test.com",
                Valor = 5
            };
            var command = new RegisterAnswerCommand(dto);
            var existingAnswer = new Answer(dto.PreguntaId, userId, EnumValue.excelente);

            _userServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(dto.email))
                .ReturnsAsync(userId);

            _answerRepositoryMock.Setup(r => r.ObtenerRespuestaPorUsuarioPreguntaEncuestaAsync(dto.EncuestaId, dto.PreguntaId, userId))
                .ReturnsAsync(existingAnswer);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenUserServiceFails()
        {
            var dto = new RegisterAnswerDto { email = "error@test.com", Valor = 1 };
            var command = new RegisterAnswerCommand(dto);

            _userServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(dto.email))
                .ThrowsAsync(new Exception("User service down"));

            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}