using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.Infrastructure.Concrete;

namespace Azimuth.Services.Concrete
{
    public class PlaylistListenedService : IPlaylistListenedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<PlaylistListener> _listenerRepository;
        private readonly UserRepository _userRepository;
        private readonly IRepository<PlaylistListened> _playlistListenedRepository;
        private readonly IRepository<Playlist> _playlistRepository;

        public PlaylistListenedService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _listenerRepository = _unitOfWork.GetRepository<PlaylistListener>();
            _playlistListenedRepository = _unitOfWork.GetRepository<PlaylistListened>();
            _userRepository = _unitOfWork.GetRepository<User>() as UserRepository;
            _playlistRepository = _unitOfWork.GetRepository<Playlist>();
        }

        public Task<int> GetListenersAmount(int id)
        {
            return Task.Run(() =>
            {
                using (_unitOfWork)
                {
                    var playlist = _playlistRepository.Get(id);
                    if (playlist == null)
                    {
                        throw new BadRequestException("Playlist with Id does not exist");
                    }
                    var dop = _playlistListenedRepository.GetOne(list => list.Playlist.Id == id);
                    if (dop == null)
                    {
                        _playlistListenedRepository.AddItem(new PlaylistListened
                        {
                            Amount = 1,
                            Playlist = playlist
                        });
                        return 1;
                    }
                    return dop.Amount;
                }
            });
        }

        public void AddNewListener(int playlistId)
        {

            using (_unitOfWork)
            {
                var playlist = _playlistRepository.Get(playlistId);
                if (playlist == null)
                {
                    throw new BadRequestException("Playlist with Id does not exist");
                }
                var listenerPlaylist =
                    _playlistListenedRepository.GetOne(listener => listener.Playlist.Id == playlist.Id);
                if (listenerPlaylist != null)
                {
                    ++listenerPlaylist.Amount;
                    _playlistListenedRepository.UpdateItem(listenerPlaylist);
                }
                else
                {
                    _playlistListenedRepository.AddItem(new PlaylistListened
                    {
                        Amount = 1,
                        Playlist = playlist
                    });
                }
                _unitOfWork.Commit();
            }
        }
    }
}