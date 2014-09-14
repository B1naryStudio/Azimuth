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
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly INotificationService _notificationService;

        public ListenersService(IUnitOfWorkFactory unitOfWorkFactory, INotificationService notificationService)
        {
            _unitOfWorkFactory = unitOfWorkFactory;

            _notificationService = notificationService;
        }

        public Task<List<User>> GetListenersByPlaylistId(int id)
        {
            return Task.Run(() =>
            {
                using (var _unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
                {
                    var _listenerRepository = _unitOfWork.GetRepository<PlaylistListener>();
                    var listeners = _listenerRepository.Get(list => list.Playlist.Id == id).Select(list=>list.Listener).ToList();
                    return listeners;
                }
                
            });
        }

        public void AddNewListener(int playlistId, int userId)
        {
            Task.Run(() =>
            {
                using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
                {
                    var _listenerRepository = unitOfWork.GetRepository<PlaylistListener>();
                    var _notificationRepository = unitOfWork.GetRepository<Notification>();

                    var user = unitOfWork.UserRepository.Get(userId);
                    if (user == null)
                    {
                        throw new BadRequestException("User with Id does not exist");
                    }

                    var playlist = unitOfWork.PlaylistRepository.Get(playlistId);
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

                    unitOfWork.Commit();
                }
            });
        }

        public void AddCurrentUserAsListener(int playlistId)
        {
            using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
            {
                var _listenerRepository = unitOfWork.GetRepository<PlaylistListener>();
                var _notificationRepository = unitOfWork.GetRepository<Notification>();

                var playlist = unitOfWork.PlaylistRepository.Get(playlistId);
                    if (playlist == null)
                    {
                        throw new BadRequestException("Playlist with Id does not exist");
                    }
                if (AzimuthIdentity.Current != null)
                {
                    var userId =
                        unitOfWork.UserRepository.GetOne(u => u.Email.Equals(AzimuthIdentity.Current.UserCredential.Email)).Id;
                    var user = unitOfWork.UserRepository.Get(userId);

                        
                    _listenerRepository.AddItem(new PlaylistListener
                    {
                        Listener = user,
                        Playlist = playlist
                    });
                }

                var notification = _notificationService.CreateNotification(Notifications.AddedNewListener, playlist.Creator, recentlyPlaylist: playlist);
                playlist.Notifications.Add(notification);
                _notificationRepository.AddItem(notification);

                unitOfWork.Commit();
            }
        }

        public void RemoveCurrentUserAsListener(int playlistId)
        {
            using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
            {
                var _listenerRepository = unitOfWork.GetRepository<PlaylistListener>();
                var _notificationRepository = unitOfWork.GetRepository<Notification>();

                var playlist = unitOfWork.PlaylistRepository.Get(playlistId);
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

                unitOfWork.Commit();
            }
        }

        
        public void RemoveListener(int playlistId, int userId)
        {
            Task.Run(() =>
            {
                using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
                {
                    var _listenerRepository = unitOfWork.GetRepository<PlaylistListener>();
                    var _notificationRepository = unitOfWork.GetRepository<Notification>();

                    var listener = _listenerRepository.GetOne(pair => pair.Playlist.Id == playlistId && pair.Listener.Id == userId);
                    if (listener == null)
                    {
                        throw new BadRequestException("This listener pair does not exist");
                    }
                    _listenerRepository.DeleteItem(listener);

                    var playlist = unitOfWork.PlaylistRepository.GetOne(p => p.Id == playlistId);

                    var notification = _notificationService.CreateNotification(Notifications.RemovedListener, playlist.Creator);
                    _notificationRepository.AddItem(notification);

                    unitOfWork.Commit();
                }
            });
        }
    }
}