using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveys_services.application.DTOs
{
    public class CompletedSurveyDto
    {
        public Guid SurveyId { get; set; }
        public Guid EventId { get; set; }
        public string SurveyTitle { get; set; }
    }
}
