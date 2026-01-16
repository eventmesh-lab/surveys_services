using Xunit;
using surveys_services.application.Queries.Queries;
using surveys_services.application.DTOs;
using System.Collections.Generic;

namespace surveys_services.tests.Application.Queries
{
    public class GetCompletedSurveysByUserQueryTests
    {
        [Fact]
        public void Constructor_ShouldInitializeEmailProperty()
        {
            var email = "usuario@test.com";

            var query = new GetCompletedSurveysByUserQuery(email);

            Assert.Equal(email, query.Email);
        }

        [Fact]
        public void Property_ShouldBeSettable()
        {
            var query = new GetCompletedSurveysByUserQuery("inicial@test.com");
            var newEmail = "nuevo@test.com";

            query.Email = newEmail;

            Assert.Equal(newEmail, query.Email);
        }

        [Fact]
        public void Query_ShouldImplementIRequestWithCorrectReturnType()
        {
            var query = new GetCompletedSurveysByUserQuery("test@test.com");

            Assert.IsAssignableFrom<MediatR.IRequest<List<CompletedSurveyDto>>>(query);
        }
    }
}