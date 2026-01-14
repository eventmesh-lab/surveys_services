using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveys_services.application.DTOs
{
    public class QuestionDtoResponse
    {
        public Guid Id { get; set; }
        public string question { get; set; }
        
    }
}
