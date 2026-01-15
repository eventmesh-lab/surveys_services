using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveys_services.domain.Constants
{
    public static class SurveyConstants
    {
        public static readonly List<string> DefaultQuestions = new List<string>
        {
            "¿Cómo calificaría la organización del evento?",
            "¿Qué le pareció el contenido presentado?",
            "¿Recomendaría este evento a un colega?",
        };
    }
}
