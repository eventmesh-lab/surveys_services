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
        public Guid UserId { get; set; }

        public GetPendingSurveysByUserQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
