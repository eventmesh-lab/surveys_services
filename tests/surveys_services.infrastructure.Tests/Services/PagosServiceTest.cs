using Moq;
using Moq.Protected;
using surveys_services.application.DTOs;
using surveys_services.infrastructure.Services;
using System.Net;
using System.Text.Json;
using Xunit;

namespace surveys_services.tests.Infrastructure.Services
{
    public class PagosServiceTests
    {
        private readonly Mock<HttpMessageHandler> _handlerMock;
        private readonly HttpClient _httpClient;
        private readonly PagosService _service;

        public PagosServiceTests()
        {
            _handlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_handlerMock.Object);
            _service = new PagosService(_httpClient);
        }

        [Fact]
        public async Task ObtenerEventosPagadosPorUsuarioAsync_ShouldReturnDistinctIds_WhenResponseIsSuccessful()
        {
            var email = "angel@ucab.edu.ve";
            var eventId = Guid.NewGuid();
            var dtoList = new List<HistorialPagoExternalDto>
            {
                new HistorialPagoExternalDto { IdEvento = eventId },
                new HistorialPagoExternalDto { IdEvento = eventId }, // Duplicado para probar Distinct
                new HistorialPagoExternalDto { IdEvento = Guid.Empty } // Debería filtrarse
            };

            var responseContent = new StringContent(JsonSerializer.Serialize(dtoList));

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

            var result = await _service.ObtenerEventosPagadosPorUsuarioAsync(email, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(eventId, result[0]);
        }

        [Fact]
        public async Task ObtenerEventosPagadosPorUsuarioAsync_ShouldReturnNull_WhenResponseIsNotSuccessful()
        {
            _handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            var result = await _service.ObtenerEventosPagadosPorUsuarioAsync("test@test.com", CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task ObtenerEventosPagadosPorUsuarioAsync_ShouldReturnNull_WhenExceptionOccurs()
        {
            _handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException());

            var result = await _service.ObtenerEventosPagadosPorUsuarioAsync("test@test.com", CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task ObtenerEventosPagadosPorUsuarioAsync_ShouldReturnEmptyList_WhenJsonIsEmptyArray()
        {
            var responseContent = new StringContent("[]");

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

            var result = await _service.ObtenerEventosPagadosPorUsuarioAsync("test@test.com", CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Constructor_ShouldThrowArgumentNullException_WhenHttpClientIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new PagosService(null));
        }
    }
}