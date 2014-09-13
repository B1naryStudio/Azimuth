using System.Collections.Generic;
using System.IdentityModel;
using System.Linq;
using System.Threading.Tasks;
using Azimuth.ApiControllers;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.Infrastructure.Concrete;
using Azimuth.Services.Interfaces;
using Azimuth.Shared.Enums;

namespace Azimuth.Services.Concrete
{
    public class ListenersService : IListenersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<PlaylistListener> _listenerRepository;
        private readonly UserRepository _userRepository;
        private readonly IRepository<Playlist> _playlistRepository;
        private readonly INotificationService _notificationService;
        private readonly NotificationRepository _notificationRepository;

        public ListenersService(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _listenerRepository = _unitOfWork.GetRepository<PlaylistListener>();
            _userRepository = _unitOfWork.GetRepository<User>() as UserRepository;
            _playlistRepository = _unitOfWork.GetRepository<Playlist>();

            _notificationService = notificationService;
            _notificationRepository = _unitOfWork.GetRepository<Notification>() as NotificationRepository;
        }

        public Task<List<User>> GetListenersByPlaylistId(int id)
        {
            return Task.Run(() =>
            {
                using (_unitOfWork)
                {
                    var listeners = _listenerRepository.Get(list => list.Playlist.Id == id).Select(list=>list.Listener).ToList();
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
                    _listenerRepository.AddItem(new PlaylistListener
                    {
                        Listener = user,
                        Playlist = playlist
                    });

                    var notification = _notificationService.CreateNotification(Notifications.AddedNewListener, playlist.Creator, recentlyPlaylist: playlist);

                    playlist.Notifications.Add(notification);
                    _notificationRepository.AddItem(notification);

                    _unitOfWork.Commit();
                }
            });
        }

        public void AddCurrentUserAsListener(int playlistId)
        {
            using (_unitOfWork)
            {
                var playlist = _playlistRepository.Get(playlistId);
                    if (playlist == null)
                    {
                        throw new BadRequestException("Playlist with Id does not exist");
                    }
                if (AzimuthIdentity.Current != null)
                {
                    var userId =
                        _userRepository.GetOne(u => u.Email.Equals(AzimuthIdentity.Current.UserCredential.Email)).Id;
                    var user = _userRepository.Get(userId);

                        
                    _listenerRepository.AddItem(new PlaylistListener
                    {
                        Listener = user,
                        Playlist = playlist
                    });
                }

                var notification = _notificationService.CreateNotification(Notifications.AddedNewListener, playlist.Creator, recentlyPlaylist: playlist);
                playlist.Notifications.Add(notification);
                _notificationRepository.AddItem(notification);

                _unitOfWork.Commit();
            }
        }

        public void RemoveCurrentUserAsListener(int playlistId)
        {
            using (_unitOfWork)
            {
                var playlist = _playlistRepository.Get(playlistId);
                if (playlist == null)
                {
                    throw new BadRequestException("Playlist with Id does not exist");
                }
                if (AzimuthIdentity.Current != null)
                {
                    var userId = AzimuthIdentity.Current.UserCredential.Id;
                    var listener = _listenerRepository.GetOne(pair => pair.Playlist.Id == playlistId && pair.Listener.Id == userId);
                    _listenerRepository.DeleteItem(listener);
                }

                var notification = _notificationService.CreateNotification(Notifications.RemovedListener, playlist.Creator);

                _notificationRepository.AddItem(notification);

                _unitOfWork.Commit();
            }
        }

        
        public void RemoveListener(int playlistId, int userId)
        {
            Task.Run(() =>
            {
                using (_unitOfWork)
                {
                    var listener = _listenerRepository.GetOne(pair => pair.Playlist.Id == playlistId && pair.Listener.Id == userId);
                    if (listener == null)
                    {
                        throw new BadRequestException("This listener pair does not exist");
                    }
                    _listenerRepository.DeleteItem(listener);

                    var playlist = _playlistRepository.GetOne(p => p.Id == playlistId);

                    var notification = _notificationService.CreateNotification(Notifications.RemovedListener, playlist.Creator);
                    _notificationRepository.AddItem(notification);

                    _unitOfWork.Commit();
                }
            });
        }
    }
}