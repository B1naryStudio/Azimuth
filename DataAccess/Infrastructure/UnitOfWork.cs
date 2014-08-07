using System;
using NHibernate;

namespace Azimuth.DataAccess.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly ISessionFactory _sessionFactory;
        private readonly IRepositoryFactory _repoFactory;
        private ISession _currentSession;
        private ITransaction _currentTransaction;

        public UnitOfWork(ISessionFactory sessionFactory, IRepositoryFactory repoFactory)
        {
            _sessionFactory = sessionFactory;
            _repoFactory = repoFactory;
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

        public void ReopenSession()
        {
            _currentSession = _sessionFactory.OpenSession();
        }

        public IRepository<T> GetRepository<T>() where T : class, IEntity
        {
            return _repoFactory.Resolve<T>(CurrentSession);
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
        }
    }
}
