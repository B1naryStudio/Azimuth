using System.Collections.Generic;
using System.IdentityModel;
using System.Linq;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.Infrastructure.Concrete;

namespace Azimuth.ApiControllers
{
    public class ListenersService : IListenersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PlaylistListenersRepository _listenersRepository;
        private readonly UserRepository _userRepository;
        private readonly PlaylistRepository _playlistRepository;

        public ListenersService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _listenersRepository = _unitOfWork.GetRepository<PlaylistListeners>() as PlaylistListenersRepository;
            _userRepository = _unitOfWork.GetRepository<User>() as UserRepository;
            _playlistRepository = _unitOfWork.GetRepository<Playlist>() as PlaylistRepository;
        }

        public Task<List<User>> GetListenersByPlaylistId(int id)
        {
            return Task.Run(() =>
            {
                using (_unitOfWork)
                {
                    var listeners = _listenersRepository.Get(list => list.Playlist.Id == id).Select(list=>list.Listener).ToList();
                    return listeners;
                }
                
            });
        }

        public void AddNewListener(int playlistId, int userId)
        {
            Task.Run(() =>
            {
                using (_unitOfWork)
                {                    
                    var user = _userRepository.Get(userId);
                    if (user == null)
                    {
                        throw new BadRequestException("User with Id does not exist");
                    }
                    
                    var playlist = _playlistRepository.Get(playlistId);
                    if (playlist == null)
                    {
                        throw new BadRequestException("Playlist with Id does not exist");
                    }
                    _listenersRepository.AddItem(new PlaylistListeners
                    {
                        Listener = user,
                        Playlist = playlist
                    });
                    _unitOfWork.Commit();
                }

            });
        }

        public void AddCurrentUserAsListener(int playlistId)
        {
            Task.Run(() =>
            {
                using (_unitOfWork)
                {

                    var userId =
                        _userRepository.GetOne(u => u.Email.Equals(AzimuthIdentity.Current.UserCredential.Email)).Id;
                    var user = _userRepository.Get(userId);
                    if (user == null)
                    {
                        throw new BadRequestException("User with Id does not exist");
                    }

                    var playlist = _playlistRepository.Get(playlistId);
                    if (playlist == null)
                    {
                        throw new BadRequestException("Playlist with Id does not exist");
                    }
                    _listenersRepository.AddItem(new PlaylistListeners
                    {
                        Listener = user,
                        Playlist = playlist
                    });
                    _unitOfWork.Commit();
                }

            });
        }

        public void RemoveListener(int playlistId, int userId)
        {
            Task.Run(() =>
            {
                using (_unitOfWork)
                {
                    var listener = _listenersRepository.GetOne(pair => pair.Playlist.Id == playlistId && pair.Listener.Id == userId);
                    if (listener == null)
                    {
                        throw new BadRequestException("This listener pair does not exist");
                    }
                    _listenersRepository.DeleteItem(listener);
                    _unitOfWork.Commit();
                }

            });
        }
    }
}