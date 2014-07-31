﻿using System.Reflection;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Repositories;
using NHibernate;
using NHibernate.Cfg;
using Ninject.Extensions.Factory;
using Ninject.Modules;

namespace Azimuth.DataAccess.Infrastructure
{
    public class DataAccessModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IRepositoryFactory>().ToFactory();
            Bind<IUnitOfWork>().To<UnitOfWork>();
            Bind<ISessionFactory>().ToMethod(ctx =>
            {
                var cfg = new Configuration();
                cfg.Configure();
                cfg.AddAssembly(Assembly.GetExecutingAssembly());
                return cfg.BuildSessionFactory();
            });
            Bind<IRepository<User>, BaseRepository<User>>().To<UserRepository>();
        }
    }

    public interface IRepositoryFactory
    {
        IRepository<T> Resolve<T>(ISession session) where T : IEntity;
    }
}
