using Xunit;
using surveys_services.application.Commands.Commands;
using surveys_services.application.DTOs;
using System;

namespace surveys_services.tests.Application.Commands
{
    public class RegisterAnswerCommandTests
    {
        [Fact]
        public void Constructor_ShouldInitializeAnswerDtoProperty()
        {
            var dto = new RegisterAnswerDto
            {
                EncuestaId = Guid.NewGuid(),
                PreguntaId = Guid.NewGuid(),
                email = "usuario@test.com",
                Valor = 5
            };

            var command = new RegisterAnswerCommand(dto);

            Assert.NotNull(command.AnswerDto);
            Assert.Equal(dto, command.AnswerDto);
            Assert.Equal("usuario@test.com", command.AnswerDto.email);
        }

        [Fact]
        public void Command_ShouldImplementIRequestWithGuidReturnType()
        {
            var dto = new RegisterAnswerDto();
            var command = new RegisterAnswerCommand(dto);

            Assert.IsAssignableFrom<MediatR.IRequest<Guid>>(command);
        }

        [Fact]
        public void Record_ShouldHaveValueEquality()
        {
            var dto = new RegisterAnswerDto { email = "test@test.com" };
            var command1 = new RegisterAnswerCommand(dto);
            var command2 = new RegisterAnswerCommand(dto);

            Assert.Equal(command1, command2);
            Assert.True(command1 == command2);
        }
    }
}