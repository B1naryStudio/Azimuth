using System.Collections.Generic;
using System.IdentityModel;
using System.Linq;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.Infrastructure.Concrete;
using Azimuth.Services.Interfaces;
using Azimuth.Shared.Enums;

namespace Azimuth.Services.Concrete
{
    public class PlaylistLikesService : IPlaylistLikesService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        //private readonly IRepository<PlaylistLike> _likerRepository;
        //private readonly UserRepository _userRepository;
        //private readonly IRepository<Playlist> _playlistRepository;
        //private readonly NotificationRepository _notificationRepository;
        private readonly INotificationService _notificationService;

        public PlaylistLikesService(IUnitOfWorkFactory unitOfWorkFactory, INotificationService notificationService)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _notificationService = notificationService;
            //_likerRepository = _unitOfWork.GetRepository<PlaylistLike>();
            //_userRepository = _unitOfWork.GetRepository<User>() as UserRepository;
            //_playlistRepository = _unitOfWork.GetRepository<Playlist>();
            //_notificationRepository = _unitOfWork.GetRepository<Notification>() as NotificationRepository;
        }

        public Task<List<User>> GetLikersByPlaylistId(int id)
        {
            return Task.Run(() =>
            {
                using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
                {
                    var _likerRepository = unitOfWork.GetRepository<PlaylistLike>();
                    var listeners = _likerRepository.Get(list => list.Playlist.Id == id && list.IsLiked).Select(list=>list.Liker).ToList();
                    return listeners;
                }
            });
        }

        public void AddCurrentUserAsLiker(int playlistId)
        {
            using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
            {
                var _likerRepository = unitOfWork.GetRepository<PlaylistLike>();
                var _notificationRepository = unitOfWork.GetRepository<Notification>();

                var playlist = unitOfWork.PlaylistRepository.Get(playlistId);
                    if (playlist == null)
                    {
                        throw new BadRequestException("Playlist with Id does not exist");
                    }
                if (AzimuthIdentity.Current != null)
                {
                    var userId = AzimuthIdentity.Current.UserCredential.Id;
                        //_userRepository.GetOne(u => u.Email.Equals(AzimuthIdentity.Current.UserCredential.Email)).Id;
                    var user = unitOfWork.UserRepository.Get(userId);


                    var atemp = _likerRepository.GetOne(t => t.Playlist.Id == playlistId && t.Liker.Id == userId);
                    if (atemp == null)
                    {
                        _likerRepository.AddItem(new PlaylistLike
                        {
                            Liker = user,
                            Playlist = playlist,
                            IsLiked = true,
                            IsFavorite = false
                        });
                    }
                        
                    else
                    {
                        atemp.IsLiked = true;
                        _likerRepository.UpdateItem(atemp);
                    }

                    var notification = _notificationService.CreateNotification(Notifications.LikedPlaylist, user, recentlyPlaylist: playlist);

                    playlist.Notifications.Add(notification);
                    _notificationRepository.AddItem(notification);
                }
                unitOfWork.Commit();
            }
        }

        public void AddCurrentUserAsFavorite(int playlistId)
        {
            using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
            {
                var _likerRepository = unitOfWork.GetRepository<PlaylistLike>();
                var _notificationRepository = unitOfWork.GetRepository<Notification>();

                var playlist = unitOfWork.PlaylistRepository.Get(playlistId);
                if (playlist == null)
                {
                    throw new BadRequestException("Playlist with Id does not exist");
                }
                if (AzimuthIdentity.Current != null)
                {
                    var userId = AzimuthIdentity.Current.UserCredential.Id;
                    var user = unitOfWork.UserRepository.Get(userId);


                    var atemp = _likerRepository.GetOne(t => t.Playlist.Id == playlistId && t.Liker.Id == userId);
                    if (atemp == null)
                    {
                        _likerRepository.AddItem(new PlaylistLike
                        {
                            Liker = user,
                            Playlist = playlist,
                            IsLiked = false,
                            IsFavorite = true
                        });
                    }
                        
                    else
                    {
                        atemp.IsFavorite = true;
                        _likerRepository.UpdateItem(atemp);
                    }

                    var notification = _notificationService.CreateNotification(Notifications.FavoritedPlaylist, user, recentlyPlaylist: playlist);
                    
                    playlist.Notifications.Add(notification);
                    _notificationRepository.AddItem(notification);
                }

                unitOfWork.Commit();
            }
        }

        public void RemoveCurrentUserAsLiker(int playlistId)
        {
            using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
            {
                var _likerRepository = unitOfWork.GetRepository<PlaylistLike>();
                var _notificationRepository = unitOfWork.GetRepository<Notification>();

                var playlist = unitOfWork.PlaylistRepository.Get(playlistId);
                if (playlist == null)
                {
                    throw new BadRequestException("Playlist with Id does not exist");
                }
                if (AzimuthIdentity.Current != null)
                {
                    var userId = AzimuthIdentity.Current.UserCredential.Id;
                     //   _userRepository.GetOne(u => u.Email.Equals(AzimuthIdentity.Current.UserCredential.Email)).Id;
                    var liker = _likerRepository.GetOne(pair => pair.Playlist.Id == playlistId && pair.Liker.Id == userId);
                    if (liker != null)
                    {
                        liker.IsLiked = false;
                    }

                    var user = unitOfWork.UserRepository.GetOne(u => u.Id == userId);

                    var notification = _notificationService.CreateNotification(Notifications.UnlikedPlaylist, user, recentlyPlaylist: playlist);

                    playlist.Notifications.Add(notification);
                    _notificationRepository.AddItem(notification);

                }

                unitOfWork.Commit();
            }
        }

        public void RemoveCurrentUserAsFavorite(int playlistId)
        {
            using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
            {
                var _likerRepository = unitOfWork.GetRepository<PlaylistLike>();
                var _notificationRepository = unitOfWork.GetRepository<Notification>();

                var playlist = unitOfWork.PlaylistRepository.Get(playlistId);
                if (playlist == null)
                {
                    throw new BadRequestException("Playlist with Id does not exist");
                }
                if (AzimuthIdentity.Current != null)
                {
                    var userId = AzimuthIdentity.Current.UserCredential.Id;
                        //_userRepository.GetOne(u => u.Email.Equals(AzimuthIdentity.Current.UserCredential.Email)).Id;
                    var liker = _likerRepository.GetOne(pair => pair.Playlist.Id == playlistId && pair.Liker.Id == userId);
                    if (liker != null)
                    {
                        liker.IsFavorite = false;
                    }

                    var user = unitOfWork.UserRepository.GetOne(u => u.Id == userId);

                    var notification = _notificationService.CreateNotification(Notifications.UnfavoritedPlaylist, user, recentlyPlaylist: playlist);

                    playlist.Notifications.Add(notification);
                    _notificationRepository.AddItem(notification);
                }
                unitOfWork.Commit();
            }
        }

        public Task<bool> IsLiked(int id)
        {
            var dop = AzimuthIdentity.Current;
            return Task.Run(() =>
            {
                using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
                {
                    var _likerRepository = unitOfWork.GetRepository<PlaylistLike>();
                    bool res = false;
                    var playlist = unitOfWork.PlaylistRepository.Get(id);
                    if (playlist == null)
                    {
                        throw new BadRequestException("Playlist with Id does not exist");
                    }
                    if (dop != null)
                    {
                        var userId = dop.UserCredential.Id;
                            //_userRepository.GetOne(u => u.Email.Equals(dop.UserCredential.Email)).Id;
                        var liker = _likerRepository.GetOne(pair => pair.Playlist.Id == id && pair.Liker.Id == userId);
                        res = (liker != null) && liker.IsLiked;
                    }
                    unitOfWork.Commit();
                    return res;
                }
            });
        }

        public Task<bool> IsFavorite(int id)
        {
            var dop = AzimuthIdentity.Current;
            return Task.Run(() =>
            {
                using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
                {
                    var _likerRepository = unitOfWork.GetRepository<PlaylistLike>();
                    bool res = false;
                    var playlist = unitOfWork.PlaylistRepository.Get(id);
                    if (playlist == null)
                    {
                        throw new BadRequestException("Playlist with Id does not exist");
                    }
                    if (dop != null)
                    {
                        var userId = dop.UserCredential.Id;
                            //_userRepository.GetOne(u => u.Email.Equals(dop.UserCredential.Email)).Id;
                        var liker = _likerRepository.GetOne(pair => pair.Playlist.Id == id && pair.Liker.Id == userId);
                        res = (liker != null) && liker.IsFavorite;
                    }
                    unitOfWork.Commit();
                    return res;
                }
            });
        }
    }
}