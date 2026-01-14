using MediatR;
using surveys_services.application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveys_services.application.Commands.Commands
{
    public record RegisterAnswerCommand(RegisterAnswerDto AnswerDto) : IRequest<Guid>;
}
