using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using surveys_services.application.DTOs;

namespace surveys_services.application.Queries.Queries
{
    public class GetUserSurveyAnswersByEventQuery : IRequest<SurveyResultByEventDto>
    {
        public string Email { get; set; }
        public Guid EventId { get; set; }

        public GetUserSurveyAnswersByEventQuery(string email, Guid eventId)
        {
            Email = email;
            EventId = eventId;
        }
    }
}
