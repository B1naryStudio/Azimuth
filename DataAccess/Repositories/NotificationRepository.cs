using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using NHibernate;

namespace Azimuth.DataAccess.Repositories
{
    public class NotificationRepository : BaseRepository<Notification>
    {
        public NotificationRepository(ISession session) : base(session)
        {
        }
    }
}