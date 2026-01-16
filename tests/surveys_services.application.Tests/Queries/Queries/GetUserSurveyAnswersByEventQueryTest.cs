using Xunit;
using surveys_services.application.Queries.Queries;
using surveys_services.application.DTOs;
using System;

namespace surveys_services.tests.Application.Queries
{
    public class GetUserSurveyAnswersByEventQueryTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            var email = "usuario@test.com";
            var eventId = Guid.NewGuid();

            var query = new GetUserSurveyAnswersByEventQuery(email, eventId);

            Assert.Equal(email, query.Email);
            Assert.Equal(eventId, query.EventId);
        }

        [Fact]
        public void Properties_ShouldBeSettable()
        {
            var query = new GetUserSurveyAnswersByEventQuery("viejo@test.com", Guid.NewGuid());
            var newEmail = "nuevo@test.com";
            var newEventId = Guid.NewGuid();

            query.Email = newEmail;
            query.EventId = newEventId;

            Assert.Equal(newEmail, query.Email);
            Assert.Equal(newEventId, query.EventId);
        }

        [Fact]
        public void Query_ShouldImplementIRequestWithCorrectReturnType()
        {
            var query = new GetUserSurveyAnswersByEventQuery("test@test.com", Guid.NewGuid());

            Assert.IsAssignableFrom<MediatR.IRequest<SurveyResultByEventDto>>(query);
        }
    }
}