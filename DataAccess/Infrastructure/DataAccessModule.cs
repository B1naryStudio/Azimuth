using System;
using System.IO;
using System.Reflection;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Repositories;
using NHibernate;
using NHibernate.Cfg;
using Ninject.Extensions.Factory;
using Ninject.Modules;
using NHibernate.Tool.hbm2ddl;

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
#if DEBUG
                cfg.Configure();
#else
                cfg.Configure(Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, "hibernate-release.cfg.xml"));
#endif
                cfg.AddAssembly(Assembly.GetExecutingAssembly());
//                var schemaExport = new SchemaExport(cfg);
//                schemaExport.Drop(false, true);
//                schemaExport.Create(false, true);
                return cfg.BuildSessionFactory();
            });
            Bind<IRepository<User>, BaseRepository<User>>().To<UserRepository>();
            Bind<IRepository<Artist>, BaseRepository<Artist>>().To<ArtistRepository>();
            Bind<IRepository<Album>, BaseRepository<Album>>().To<AlbumRepository>();
            Bind<IRepository<Track>, BaseRepository<Track>>().To<TrackRepository>();
            Bind<IRepository<Playlist>, BaseRepository<Playlist>>().To<PlaylistRepository>();
            Bind<IRepository<SocialNetwork>, BaseRepository<SocialNetwork>>().To<SocialNetworkRepository>();
            Bind<IRepository<UserSocialNetwork>, BaseRepository<UserSocialNetwork>>().To<UserSocialNetworkRepository>();
        }
    }

    public interface IRepositoryFactory
    {
        IRepository<T> Resolve<T>(ISession session) where T : IEntity;
    }
}
