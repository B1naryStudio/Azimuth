using System;

namespace Azimuth.DataAccess.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> GetRepository<T>() where T : class, IEntity;
        void Commit();
        void Rollback();
    }
}
