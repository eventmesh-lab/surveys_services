using Moq;
using Moq.Protected;
using surveys_services.infrastructure.Services;
using System.Net;
using Xunit;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace surveys_services.tests.Infrastructure.Services
{
    public class UserServiceTests
    {
        private readonly Mock<HttpMessageHandler> _handlerMock;
        private readonly HttpClient _httpClient;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _handlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_handlerMock.Object);
            _service = new UserService(_httpClient);
        }

        [Fact]
        public async Task ObtenerUsuarioPorEmailAsync_ShouldReturnGuid_WhenResponseIsSuccessfulAndValid()
        {
            var email = "test@ucab.edu.ve";
            var expectedGuid = Guid.NewGuid();
            var responseContent = new StringContent($"\"{expectedGuid}\"");

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

            var result = await _service.ObtenerUsuarioPorEmailAsync(email);

            Assert.Equal(expectedGuid, result);
        }

        [Fact]
        public async Task ObtenerUsuarioPorEmailAsync_ShouldReturnEmptyGuid_WhenResponseIsNotFound()
        {
            var email = "notfound@test.com";

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

            var result = await _service.ObtenerUsuarioPorEmailAsync(email);

            Assert.Equal(Guid.Empty, result);
        }

        [Fact]
        public async Task ObtenerUsuarioPorEmailAsync_ShouldReturnEmptyGuid_WhenContentIsInvalidGuid()
        {
            var email = "invalid@test.com";
            var responseContent = new StringContent("not-a-guid");

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

            var result = await _service.ObtenerUsuarioPorEmailAsync(email);

            Assert.Equal(Guid.Empty, result);
        }

        [Fact]
        public async Task ObtenerUsuarioPorEmailAsync_ShouldHandleGuidsWithQuotesCorrectly()
        {
            var email = "quoted@test.com";
            var expectedGuid = Guid.NewGuid();
            var responseContent = new StringContent($"\"{expectedGuid}\"");

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

            var result = await _service.ObtenerUsuarioPorEmailAsync(email);

            Assert.Equal(expectedGuid, result);
        }
    }
}