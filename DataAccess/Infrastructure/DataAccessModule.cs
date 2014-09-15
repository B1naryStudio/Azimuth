using System;
using System.IO;
using System.Reflection;
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
            Bind<IUnitOfWorkFactory>().ToFactory();
            Bind<IRepositoryFactory>().ToFactory();
            Bind<IUnitOfWork>().To<UnitOfWork>();
            Bind<ISessionFactory>().ToMethod(ctx =>
            {
                var cfg = new Configuration();
#if DEBUG
                cfg.Configure();
#else
                cfg.Configure(Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, "hibernate.cfg.xml"));
#endif
                cfg.AddAssembly(Assembly.GetExecutingAssembly());
                return cfg.BuildSessionFactory();
            });
            Bind<IRepository<User>, IUserRepository>().To<UserRepository>();
            Bind<IRepository<Artist>>().To<ArtistRepository>();
            Bind<IRepository<Album>>().To<AlbumRepository>();
            Bind<IRepository<Track>>().To<TrackRepository>();
            Bind<IRepository<Playlist>, IPlaylistRepository>().To<PlaylistRepository>();
            Bind<IRepository<SocialNetwork>, ISocialNetworkRepository>().To<SocialNetworkRepository>();
            Bind<IRepository<UserSocialNetwork>, IUserSocialNetworkRepository>().To<UserSocialNetworkRepository>();
            Bind<IRepository<PlaylistTrack>>().To<PlaylistTrackRepository>();
            Bind<IRepository<PlaylistListener>>().To<PlaylistListenersRepository>();
            Bind<IRepository<SharedPlaylist>>().To<SharedPlaylistRepository>();
            Bind<IRepository<PlaylistLike>>().To<PlaylistLikerRepository>();
            Bind<IRepository<Notification>, INotificationRepository>().To<NotificationRepository>();
            Bind<IRepository<PlaylistLike>, IPlaylistLikerRepository>().To<PlaylistLikerRepository>();
        }
    }

    public interface IRepositoryFactory
    {
        IRepository<T> Resolve<T>(ISession session) where T : IEntity;

        T ResolveTyped<T>(ISession session);
    }

    public interface IUnitOfWorkFactory
    {
        IUnitOfWork NewUnitOfWork();
    }
}
