using MediatR;
using surveys_services.application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveys_services.application.Queries.Queries
{
    public class GetCompletedSurveysByUserQuery : IRequest<List<CompletedSurveyDto>>
    {
        public string Email { get; set; }

        public GetCompletedSurveysByUserQuery(string email)
        {
            Email = email;
        }
    }
}
