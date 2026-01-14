using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveys_services.infrastructure.Persistence.Models
{
    public class QuestionPostgres
    {
        public Guid Id { get; set; }
        public Guid IdEncuesta { get; set; }
        public string Text { get; set; }
    }
}
