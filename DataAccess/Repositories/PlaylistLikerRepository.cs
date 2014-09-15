using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using NHibernate;

namespace Azimuth.DataAccess.Repositories
{
    public class PlaylistLikerRepository:BaseRepository<PlaylistLike>, IPlaylistLikerRepository
    {
        public PlaylistLikerRepository(ISession session) : base(session)
        {
        }
    }

    public interface IPlaylistLikerRepository : IRepository<PlaylistLike>
    {
        
    }
}
