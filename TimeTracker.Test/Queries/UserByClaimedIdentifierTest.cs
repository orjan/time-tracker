using System;
using System.Linq;
using FluentAssertions;
using NodaTime;
using Raven.Client;
using Raven.Client.Linq;
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
                User user =
                    session.Query<User>("UserByClaimedIdentifier")
                           .SingleOrDefault(u => u.ClaimedIdentifier.Equals("orjansjoholm@gmail.com"));
                user.Should().NotBeNull();
                Assert.Equal("orjansjoholm@gmail.com", user.ClaimedIdentifier);
            }
        }


        [Fact]
        public void Should_be_able_to_calculate_time()
        {
            using (IDocumentSession session = DocumentStore.OpenSession())
            {
                session.Store(Entity(LocalTime.Noon, TimeSpan.FromHours(2)));
                session.Store(Entity(LocalTime.Noon.PlusHours(1), TimeSpan.FromHours(2)));
                session.SaveChanges();
            }

            using (IDocumentSession session = DocumentStore.OpenSession())
            {
                var queryable = session.Query<TimeLog>().ToList()
                                       .GroupBy(g => new {g.UserId, g.StartTime.Date})
                                       .Select((g, y) => new
                                                             {
                                                                 g.Key.UserId,
                                                                 g.Key.Date,
                                                                 TotalTime = new TimeSpan(g.Sum(x => x.Duration.Ticks))
                                                             });

                Assert.Equal(TimeSpan.FromHours(4), queryable.First().TotalTime);
            }
        }

        [Fact]
        public void Should_be_able_to_calculate_time_reduce()
        {
            DocumentStore.ExecuteIndex(new TotalWorkByUserAndDay());

            using (IDocumentSession session = DocumentStore.OpenSession())
            {
                session.Store(Entity(LocalTime.Noon, TimeSpan.FromHours(2)));
                session.Store(Entity(LocalTime.Noon.PlusHours(2), TimeSpan.FromHours(2)));
                session.SaveChanges();
            }

            using (IDocumentSession session = DocumentStore.OpenSession())
            {
                IRavenQueryable<TotalWorkByUserAndDay.Result> query =
                    session.Query<TotalWorkByUserAndDay.Result, TotalWorkByUserAndDay>();

                Assert.Equal(TimeSpan.FromHours(4), query.First().TotalAmountOfWork);
            }
        }

        private DateTimeOffset CreateTime(LocalTime localTime)
        {
            DateTimeZone dateTimeZone = DateTimeZoneProviders.Default["Europe/Stockholm"];
            var localDate = new LocalDate(2013, 3, 4);
            return (localDate + localTime).InZoneStrictly(dateTimeZone).ToDateTimeOffset();
        }

        private TimeLog Entity(LocalTime startDate, TimeSpan time)
        {
            return new TimeLog
                       {
                           UserId = 1,
                           StartTime = CreateTime(startDate),
                           Duration = time
                       };
        }
    }
}