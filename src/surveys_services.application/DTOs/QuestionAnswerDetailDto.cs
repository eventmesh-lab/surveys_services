using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveys_services.application.DTOs
{
    public class QuestionAnswerDetailDto
    {
        public Guid QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string AnswerValue { get; set; } 
        public DateTime? AnswerDate { get; set; }
    }
}
