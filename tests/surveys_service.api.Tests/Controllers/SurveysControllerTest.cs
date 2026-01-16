using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using surveys_services.api.Controllers;
using surveys_services.application.Commands.Commands;
using surveys_services.application.DTOs;
using surveys_services.application.Queries.Queries;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace surveys_services.tests.Api
{
    public class SurveysControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly SurveysController _controller;

        public SurveysControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new SurveysController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetPendingSurveys_ShouldReturnOk_WhenEmailIsValid()
        {
            var email = "test@test.com";
            var expectedList = new List<PendingSurveyDto> { new PendingSurveyDto() };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPendingSurveysByUserQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedList);

            var result = await _controller.GetPendingSurveys(email);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedList, okResult.Value);
        }

        [Fact]
        public async Task GetPendingSurveys_ShouldReturnBadRequest_WhenEmailIsEmpty()
        {
            var result = await _controller.GetPendingSurveys("");

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("El email del usuario no es válido.", badRequest.Value);
        }

        [Fact]
        public async Task RegisterAnswer_ShouldReturnOk_WhenCommandSucceeds()
        {
            var dto = new RegisterAnswerDto
            {
                EncuestaId = Guid.NewGuid(),
                PreguntaId = Guid.NewGuid(),
                email = "user@test.com",
                Valor = 5
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<RegisterAnswerCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid);

            var result = await _controller.RegisterAnswer(dto, CancellationToken.None);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task RegisterAnswer_ShouldReturnConflict_WhenInvalidOperationExceptionOccurs()
        {
            var dto = new RegisterAnswerDto();
            _mediatorMock.Setup(m => m.Send(It.IsAny<RegisterAnswerCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Encuesta ya respondida"));

            var result = await _controller.RegisterAnswer(dto, CancellationToken.None);

            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("Encuesta ya respondida", conflictResult.Value);
        }

        [Fact]
        public async Task RegisterAnswer_ShouldReturnBadRequest_WhenArgumentExceptionOccurs()
        {
            var dto = new RegisterAnswerDto();
            _mediatorMock.Setup(m => m.Send(It.IsAny<RegisterAnswerCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentException("Datos inválidos"));

            var result = await _controller.RegisterAnswer(dto, CancellationToken.None);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Datos inválidos", badRequest.Value);
        }

        [Fact]
        public async Task GetSurveyByEventAndUser_ShouldReturnNotFound_WhenResultIsNull()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserSurveyAnswersByEventQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((SurveyResultByEventDto)null);

            var result = await _controller.GetSurveyByEventAndUser(Guid.NewGuid(), "test@test.com");

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("No se encontró una encuesta asociada a este evento.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetSurveyStructure_ShouldReturnOk_WhenSurveyExists()
        {
            var surveyId = Guid.NewGuid();
            var expectedResponse = new SurveyAndQuestionDtoResponse
            {
                idSurvey = surveyId,
                Titulo = "Test",
                questions = new List<QuestionDtoResponse>()
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetDetailSurveyAndQuestionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var result = await _controller.GetSurveyStructure(surveyId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedResponse, okResult.Value);
        }

        [Fact]
        public async Task GetEventSurveyStats_ShouldReturnBadRequest_WhenIdIsEmpty()
        {
            var result = await _controller.GetEventSurveyStats(Guid.Empty);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("El ID del evento no es válido.", badRequest.Value);
        }

        [Fact]
        public async Task GetCompletedSurveys_ShouldReturnOk_WithList()
        {
            var email = "test@test.com";
            var expectedList = new List<CompletedSurveyDto> { new CompletedSurveyDto() };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCompletedSurveysByUserQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedList);

            var result = await _controller.GetCompletedSurveys(email);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedList, okResult.Value);
        }

        [Fact]
        public async Task RegisterAnswer_ShouldReturnInternalServerError_WhenGenericExceptionOccurs()
        {
            var dto = new RegisterAnswerDto();
            _mediatorMock.Setup(m => m.Send(It.IsAny<RegisterAnswerCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database Error"));

            var result = await _controller.RegisterAnswer(dto, CancellationToken.None);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("Error interno: Database Error", objectResult.Value);
        }
    }
}