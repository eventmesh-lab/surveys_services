using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveys_services.domain.Entities
{
    public class Question
    {
        public Guid Id { get; set; } 
        public Guid IdEncuesta { get; set; }
        public string Text { get; set; }

        public Question(Guid idEncuesta, string text)
        {
            Id = Guid.NewGuid();
            IdEncuesta = idEncuesta;
            Text = text;
        }

        public Question(Guid id, Guid idEncuesta, string text)
        {
            Id =id;
            IdEncuesta = idEncuesta;
            Text = text;
        }
    }
}
