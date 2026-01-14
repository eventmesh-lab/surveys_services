using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveys_services.application.DTOs
{
    public class SurveyResultByEventDto
    {
        public Guid SurveyId { get; set; }
        public string SurveyTitle { get; set; }
        public Guid EventId { get; set; }
        public List<QuestionAnswerDetailDto> Details { get; set; } = new List<QuestionAnswerDetailDto>();
    }
}
