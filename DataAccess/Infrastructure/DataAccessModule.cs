using System.Reflection;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Repositories;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
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
//                var schemaExport = new SchemaExport(cfg);
//                schemaExport.Create(false, true);
                return cfg.BuildSessionFactory();
            });
            Bind<IRepository<User>, BaseRepository<User>>().To<UserRepository>();
            Bind<IRepository<SocialNetwork>, BaseRepository<SocialNetwork>>().To<SocialNetworkRepository>();
            Bind<IRepository<UserSocialNetwork>, BaseRepository<UserSocialNetwork>>().To<UserSocialNetworkRepository>();
        }
    }

    public interface IRepositoryFactory
    {
        IRepository<T> Resolve<T>(ISession session) where T : IEntity;
    }
}
