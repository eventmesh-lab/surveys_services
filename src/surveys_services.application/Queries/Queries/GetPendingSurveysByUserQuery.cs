using MediatR;
using surveys_services.application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveys_services.application.Queries.Queries
{
    public class GetPendingSurveysByUserQuery : IRequest<List<PendingSurveyDto>>
    {
        public string Email { get; set; }

        public GetPendingSurveysByUserQuery(string email)
        {
            Email = email;
        }
    }
}
