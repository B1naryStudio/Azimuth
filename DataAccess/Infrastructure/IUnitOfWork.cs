using System;
using Azimuth.DataAccess.Repositories;

namespace Azimuth.DataAccess.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> GetRepository<T>() where T : class, IEntity;

        T GetTypedRepository<T>();

        IUserRepository UserRepository { get; }
        IPlaylistRepository PlaylistRepository { get; }
        INotificationRepository NotificationRepository { get; }

        void Commit();
        void Rollback();
    }
}
