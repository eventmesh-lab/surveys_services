using MediatR;
using surveys_services.application.Commands.Commands;
using surveys_services.application.Interfaces;
using surveys_services.domain.Entities;
using surveys_services.domain.Enums;
using surveys_services.domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveys_services.application.Commands.Handlers
{
    public class RegisterAnswerHandler : IRequestHandler<RegisterAnswerCommand, Guid>
    {
        private readonly IAnswerRepository _answerRepository;
        private readonly IUserService _userService;

        public RegisterAnswerHandler(IAnswerRepository answerRepository,  IUserService userService)
        {
            _answerRepository = answerRepository ?? throw new ArgumentNullException(nameof(answerRepository));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<Guid> Handle(RegisterAnswerCommand request, CancellationToken cancellationToken)
        {
            if (!Enum.IsDefined(typeof(EnumValue), request.AnswerDto.Valor))
            {
                throw new ArgumentException($"El valor '{request.AnswerDto.Valor}' no es válido. Permitido: 1-5.");
            }
            var UserId = await _userService.ObtenerUsuarioPorEmailAsync(request.AnswerDto.email);

            var existingAnswer = await _answerRepository.ObtenerRespuestaPorUsuarioPreguntaEncuestaAsync(
                request.AnswerDto.EncuestaId,
                request.AnswerDto.PreguntaId,
                UserId
            );

            if (existingAnswer != null)
            {
                throw new InvalidOperationException($"El usuario {request.AnswerDto.email} ya ha respondido la pregunta {request.AnswerDto.PreguntaId} en la encuesta {request.AnswerDto.EncuestaId}.");
            }

            var enumValor = (EnumValue)request.AnswerDto.Valor;

            var newAnswer = new Answer(
                request.AnswerDto.PreguntaId,
                UserId,
                enumValor
            );

            await _answerRepository.AddUAnswernPostgres(newAnswer, cancellationToken);

            return newAnswer.Id;
        }
    }
}
