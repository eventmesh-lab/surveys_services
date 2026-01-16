using Moq;
using surveys_services.application.DTOs;
using surveys_services.application.Interfaces;
using surveys_services.application.Queries.Handlers;
using surveys_services.application.Queries.Queries;
using surveys_services.domain.Entities;
using surveys_services.domain.Interfaces;
using surveys_services.domain.Constants;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace surveys_services.tests.Application.Handlers
{
    public class GetPendingSurveysByUserHandlerTests
    {
        private readonly Mock<IPagosService> _pagosServiceMock;
        private readonly Mock<IEventosService> _eventosServiceMock;
        private readonly Mock<ISurveysRepository> _surveysRepositoryMock;
        private readonly Mock<IQuestionRepository> _questionRepositoryMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly GetPendingSurveysByUserHandler _handler;

        public GetPendingSurveysByUserHandlerTests()
        {
            _pagosServiceMock = new Mock<IPagosService>();
            _eventosServiceMock = new Mock<IEventosService>();
            _surveysRepositoryMock = new Mock<ISurveysRepository>();
            _questionRepositoryMock = new Mock<IQuestionRepository>();
            _userServiceMock = new Mock<IUserService>();

            _handler = new GetPendingSurveysByUserHandler(
                _pagosServiceMock.Object,
                _eventosServiceMock.Object,
                _surveysRepositoryMock.Object,
                _questionRepositoryMock.Object,
                _userServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnPendingSurveys_WhenUserHasUnansweredSurveys()
        {
            var email = "test@user.com";
            var userId = Guid.NewGuid();
            var eventoId = Guid.NewGuid();
            var query = new GetPendingSurveysByUserQuery(email);
            var evento = new Evento(eventoId, "Concierto Rock", "Publicado");

            _userServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(email))
                .ReturnsAsync(userId);

            _pagosServiceMock.Setup(s => s.ObtenerEventosPagadosPorUsuarioAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Guid> { eventoId });

            _eventosServiceMock.Setup(s => s.ObtenerEstadoEventoAsync(eventoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(evento);

            var survey = new Survey(eventoId, "Encuesta de Satisfacción");
            _surveysRepositoryMock.Setup(r => r.ObtenerEncuestaPorEventoAsync(eventoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(survey);

            _surveysRepositoryMock.Setup(r => r.VerificarSiUsuarioRespondioAsync(survey.Id, userId))
                .ReturnsAsync(false);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Single(result);
            Assert.Equal(survey.Id, result[0].Id);
            Assert.Equal("Encuesta de Satisfacción", result[0].Titulo);
        }

        [Fact]
        public async Task Handle_ShouldCreateSurveyAndQuestions_WhenSurveyDoesNotExist()
        {
            var email = "new@user.com";
            var userId = Guid.NewGuid();
            var eventoId = Guid.NewGuid();
            var query = new GetPendingSurveysByUserQuery(email);
            var evento = new Evento(eventoId, "Evento Nuevo", "Publicado");

            _userServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(email)).ReturnsAsync(userId);
            _pagosServiceMock.Setup(s => s.ObtenerEventosPagadosPorUsuarioAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Guid> { eventoId });
            _eventosServiceMock.Setup(s => s.ObtenerEstadoEventoAsync(eventoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(evento);

            _surveysRepositoryMock.Setup(r => r.ObtenerEncuestaPorEventoAsync(eventoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Survey)null);

            _surveysRepositoryMock.Setup(r => r.VerificarSiUsuarioRespondioAsync(It.IsAny<Guid>(), userId))
                .ReturnsAsync(false);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotEmpty(result);
            _surveysRepositoryMock.Verify(r => r.CrearEncuestaAsync(It.IsAny<Survey>(), It.IsAny<CancellationToken>()), Times.Once);
            _questionRepositoryMock.Verify(r => r.CrearPreguntaAsync(It.IsAny<Question>(), It.IsAny<CancellationToken>()), Times.Exactly(SurveyConstants.DefaultQuestions.Count));
        }

        [Fact]
        public async Task Handle_ShouldThrowApplicationException_WhenNoPaidEventsFound()
        {
            var email = "nopayments@test.com";
            var query = new GetPendingSurveysByUserQuery(email);

            _pagosServiceMock.Setup(s => s.ObtenerEventosPagadosPorUsuarioAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Guid>());

            await Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowKeyNotFoundException_WhenAllSurveysAreAlreadyAnswered()
        {
            var email = "done@test.com";
            var userId = Guid.NewGuid();
            var eventoId = Guid.NewGuid();
            var query = new GetPendingSurveysByUserQuery(email);
            var evento = new Evento(eventoId, "Evento", "Publicado");

            _userServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(email)).ReturnsAsync(userId);
            _pagosServiceMock.Setup(s => s.ObtenerEventosPagadosPorUsuarioAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Guid> { eventoId });
            _eventosServiceMock.Setup(s => s.ObtenerEstadoEventoAsync(eventoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(evento);

            var survey = new Survey(eventoId, "Encuesta");
            _surveysRepositoryMock.Setup(r => r.ObtenerEncuestaPorEventoAsync(eventoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(survey);

            _surveysRepositoryMock.Setup(r => r.VerificarSiUsuarioRespondioAsync(survey.Id, userId))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldSkipEvent_WhenEventStateIsNotPublicado()
        {
            var email = "test@user.com";
            var userId = Guid.NewGuid();
            var eventoId = Guid.NewGuid();
            var query = new GetPendingSurveysByUserQuery(email);
            var evento = new Evento(eventoId, "Evento Privado", "Borrador");

            _userServiceMock.Setup(s => s.ObtenerUsuarioPorEmailAsync(email)).ReturnsAsync(userId);
            _pagosServiceMock.Setup(s => s.ObtenerEventosPagadosPorUsuarioAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Guid> { eventoId });
            _eventosServiceMock.Setup(s => s.ObtenerEstadoEventoAsync(eventoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(evento);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(query, CancellationToken.None));
        }
    }
}