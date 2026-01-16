using Xunit;
using surveys_services.application.Queries.Queries;
using surveys_services.application.DTOs;
using System.Collections.Generic;

namespace surveys_services.tests.Application.Queries
{
    public class GetPendingSurveysByUserQueryTests
    {
        [Fact]
        public void Constructor_ShouldInitializeEmailProperty()
        {
            var email = "user@test.com";

            var query = new GetPendingSurveysByUserQuery(email);

            Assert.Equal(email, query.Email);
        }

        [Fact]
        public void Property_ShouldBeSettable()
        {
            var query = new GetPendingSurveysByUserQuery("test@test.com");
            var newEmail = "updated@test.com";

            query.Email = newEmail;

            Assert.Equal(newEmail, query.Email);
        }

        [Fact]
        public void Query_ShouldImplementIRequestWithCorrectReturnType()
        {
            var query = new GetPendingSurveysByUserQuery("test@test.com");

            Assert.IsAssignableFrom<MediatR.IRequest<List<PendingSurveyDto>>>(query);
        }
    }
}