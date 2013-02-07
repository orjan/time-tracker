using System.Linq;
using FluentAssertions;
using Raven.Client;
using TimeTracker.Indexes;
using TimeTracker.Models;
using Xunit;

namespace TimeTracker.Queries
{
    public class UserByClaimedIdentifierTest : DocumentTestBase
    {
        [Fact]
        public void Should_be_able_to_find_a_user_by_claimed_identifier()
        {
            DocumentStore.ExecuteIndex(new UserByClaimedIdentifier());


            using (IDocumentSession session = DocumentStore.OpenSession())
            {
                var user = new User
                               {
                                   ClaimedIdentifier = "orjansjoholm@gmail.com"
                               };
                session.Store(user);
                session.SaveChanges();
            }

            using (IDocumentSession session = DocumentStore.OpenSession())
            {
                User user = session.Query<User>("UserByClaimedIdentifier").SingleOrDefault(u => u.ClaimedIdentifier.Equals("orjansjoholm@gmail.com"));
                user.Should().NotBeNull();
                Assert.Equal("orjansjoholm@gmail.com", user.ClaimedIdentifier);
            }
        }
    }
}