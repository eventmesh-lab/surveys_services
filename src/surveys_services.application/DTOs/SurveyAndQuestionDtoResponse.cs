using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveys_services.application.DTOs
{
    public class SurveyAndQuestionDtoResponse
    {
        public Guid idSurvey { get; set; }
        public string Titulo { get; set; }
        public List<QuestionDtoResponse> questions { get; set; } = new List<QuestionDtoResponse>();
    }
}
