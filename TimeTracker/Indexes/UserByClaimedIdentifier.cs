using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client.Indexes;
using TimeTracker.Models;

namespace TimeTracker.Indexes
{
    public class UserByClaimedIdentifier : AbstractIndexCreationTask<User, User>
    {
        public UserByClaimedIdentifier()
        {
            Map = users => from user in users select new {user.ClaimedIdentifier};
        }
    }
}