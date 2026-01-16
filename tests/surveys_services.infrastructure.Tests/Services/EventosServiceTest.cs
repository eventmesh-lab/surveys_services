using Moq;
using Moq.Protected;
using surveys_services.application.DTOs;
using surveys_services.domain.Entities;
using surveys_services.infrastructure.Services;
using System.Net;
using System.Text.Json;
using Xunit;

namespace surveys_services.tests.Infrastructure.Services
{
    public class EventosServiceTests
    {
        private readonly Mock<HttpMessageHandler> _handlerMock;
        private readonly HttpClient _httpClient;
        private readonly EventosService _service;

        public EventosServiceTests()
        {
            _handlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_handlerMock.Object);
            _service = new EventosService(_httpClient);
        }

        [Fact]
        public async Task ObtenerEstadoEventoAsync_ShouldReturnEvento_WhenResponseIsSuccessful()
        {
            var eventoId = Guid.NewGuid();
            var eventoDto = new EventoDto
            {
                Id = eventoId,
                Nombre = "Evento Test",
                Estado = "Publicado"
            };

            var responseContent = new StringContent(JsonSerializer.Serialize(eventoDto));

            _handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = responseContent
                });

            var result = await _service.ObtenerEstadoEventoAsync(eventoId, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(eventoId, result.Id);
            Assert.Equal("Evento Test", result.Nombre);
            Assert.Equal("Publicado", result.Estado);
        }

        [Fact]
        public async Task ObtenerEstadoEventoAsync_ShouldReturnNull_WhenResponseIsNotFound()
        {
            var eventoId = Guid.NewGuid();

            _handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            var result = await _service.ObtenerEstadoEventoAsync(eventoId, CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task ObtenerEstadoEventoAsync_ShouldThrowArgumentException_WhenAnExceptionOccurs()
        {
            var eventoId = Guid.NewGuid();

            _handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException());

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.ObtenerEstadoEventoAsync(eventoId, CancellationToken.None));

            Assert.Equal("Ocurrio un error al obtener el evento", exception.Message);
        }

        [Fact]
        public async Task Constructor_ShouldThrowArgumentNullException_WhenHttpClientIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new EventosService(null));
        }
    }
}