using Xunit;
using surveys_services.application.Queries.Queries;
using surveys_services.application.DTOs;
using System;

namespace surveys_services.tests.Application.Queries
{
    public class PromedioEncuestaPorEventoQueryTests
    {
        [Fact]
        public void Constructor_ShouldInitializeEventoIdProperty()
        {
            var eventId = Guid.NewGuid();

            var query = new PromedioEncuestaPorEventoQuery(eventId);

            Assert.Equal(eventId, query.EventoId);
        }

        [Fact]
        public void Property_ShouldBeSettable()
        {
            var query = new PromedioEncuestaPorEventoQuery(Guid.NewGuid());
            var newEventId = Guid.NewGuid();

            query.EventoId = newEventId;

            Assert.Equal(newEventId, query.EventoId);
        }

        [Fact]
        public void Query_ShouldImplementIRequestWithCorrectReturnType()
        {
            var query = new PromedioEncuestaPorEventoQuery(Guid.NewGuid());

            Assert.IsAssignableFrom<MediatR.IRequest<PromedioEventSurveyDto>>(query);
        }
    }
}