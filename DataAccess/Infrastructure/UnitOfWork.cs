using System;
using Azimuth.DataAccess.Repositories;
using NHibernate;

namespace Azimuth.DataAccess.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly ISessionFactory _sessionFactory;
        private readonly IRepositoryFactory _repoFactory;
        private ISession _currentSession;
        private ITransaction _currentTransaction;
        private Lazy<IUserRepository> _lazyUserRepo;
        private Lazy<IPlaylistRepository> _lazyPlaylistRepo;
        private Lazy<INotificationRepository> _lazyNotificationRepo;

        public UnitOfWork(ISessionFactory sessionFactory, IRepositoryFactory repoFactory)
        {
            _sessionFactory = sessionFactory;
            _repoFactory = repoFactory;

            _lazyUserRepo = new Lazy<IUserRepository>(() => GetTypedRepository<IUserRepository>());
            _lazyPlaylistRepo = new Lazy<IPlaylistRepository>(() => GetTypedRepository<IPlaylistRepository>());
            _lazyNotificationRepo = new Lazy<INotificationRepository>(() => GetTypedRepository<INotificationRepository>());
        }

        public ISession CurrentSession
        {
            get
            {
                if (_currentSession == null)
                {
                    _currentSession = _sessionFactory.OpenSession();
                    _currentTransaction = _currentSession.BeginTransaction();
                }
                return _currentSession;
            }
        }

        public IRepository<T> GetRepository<T>() where T : class, IEntity
        {
            return _repoFactory.Resolve<T>(CurrentSession);
        }

        public T GetTypedRepository<T>()
        {
            return _repoFactory.ResolveTyped<T>(CurrentSession);
        }

        public IUserRepository UserRepository
        {
            get { return _lazyUserRepo.Value; }
        }

        public IPlaylistRepository PlaylistRepository
        {
            get { return _lazyPlaylistRepo.Value; }
        }

        public INotificationRepository NotificationRepository
        {
            get { return _lazyNotificationRepo.Value; }
        }

        public void Commit()
        {
            _currentTransaction.Commit();
        }

        public void Rollback()
        {
            _currentTransaction.Rollback();
        }

        private void CloseSession()
        {
            if (_currentSession != null && _currentSession.IsOpen)
            {
                _currentSession.Flush();
                _currentSession.Close();
            }
        }

        public void Dispose()
        {
            if (_currentTransaction != null)
                _currentTransaction.Dispose();
            if (_currentSession != null)
            {
                CloseSession();
                _currentSession.Dispose();
            }
            if (_sessionFactory != null)
            {
                _sessionFactory.Dispose();
            }
        }
    }
}
