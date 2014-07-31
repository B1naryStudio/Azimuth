using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;

namespace Azimuth.DataAccess.Infrastructure
{
    public abstract class BaseRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly ISession _session;

        protected BaseRepository(ISession session)
        {
            _session = session;
        }

        public T Get(long id)
        {
            return _session.Get<T>(id);
        }

        public IEnumerable<T> Get(Func<T, bool> filter)
        {
            return _session.Query<T>().Where(filter);
        }

        public IEnumerable<T> GetAll()
        {
            return _session.Query<T>();
        }
    }
}