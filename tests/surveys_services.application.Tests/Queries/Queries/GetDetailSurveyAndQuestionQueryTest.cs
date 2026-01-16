using Xunit;
using surveys_services.application.Queries.Queries;
using surveys_services.application.DTOs;
using System;

namespace surveys_services.tests.Application.Queries
{
    public class GetDetailSurveyAndQuestionQueryTests
    {
        [Fact]
        public void Constructor_ShouldInitializeIdSurveyProperty()
        {
            var surveyId = Guid.NewGuid();

            var query = new GetDetailSurveyAndQuestionQuery(surveyId);

            Assert.Equal(surveyId, query.idSurvey);
        }

        [Fact]
        public void Property_ShouldBeSettable()
        {
            var query = new GetDetailSurveyAndQuestionQuery(Guid.NewGuid());
            var newId = Guid.NewGuid();

            query.idSurvey = newId;

            Assert.Equal(newId, query.idSurvey);
        }

        [Fact]
        public void Query_ShouldImplementIRequestWithCorrectReturnType()
        {
            var query = new GetDetailSurveyAndQuestionQuery(Guid.NewGuid());

            Assert.IsAssignableFrom<MediatR.IRequest<SurveyAndQuestionDtoResponse>>(query);
        }
    }
}