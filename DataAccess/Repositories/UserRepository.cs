﻿using System.Linq;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using NHibernate;
using NHibernate.Linq;

namespace Azimuth.DataAccess.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(ISession session) : base(session)
        {
        }

        public User GetUserByEmail(string email)
        {
            return _session.Query<User>()
                .Where(x => x.Email == email)
                .Fetch(x => x.SocialNetworks)
                .AsEnumerable()
                .FirstOrDefault();
        }
    }
}
