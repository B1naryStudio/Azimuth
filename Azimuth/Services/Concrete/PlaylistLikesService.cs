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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<PlaylistLike> _likerRepository;
        private readonly UserRepository _userRepository;
        private readonly IRepository<Playlist> _playlistRepository;
        private readonly INotificationService _notificationService;

        public PlaylistLikesService(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _likerRepository = _unitOfWork.GetRepository<PlaylistLike>();
            _userRepository = _unitOfWork.GetRepository<User>() as UserRepository;
            _playlistRepository = _unitOfWork.GetRepository<Playlist>();
            _notificationService = notificationService;
        }

        public Task<List<User>> GetLikersByPlaylistId(int id)
        {
            return Task.Run(() =>
            {
                using (_unitOfWork)
                {
                    var listeners = _likerRepository.Get(list => list.Playlist.Id == id && list.IsLiked).Select(list=>list.Liker).ToList();
                    return listeners;
                }
                
            });
        }

        public void AddCurrentUserAsLiker(int playlistId)
        {
            User user = null;
            Playlist playlist;
            using (_unitOfWork)
            {
                playlist = _playlistRepository.Get(playlistId);
                    if (playlist == null)
                    {
                        throw new BadRequestException("Playlist with Id does not exist");
                    }
                if (AzimuthIdentity.Current != null)
                {
                    var userId = AzimuthIdentity.Current.UserCredential.Id;
                        //_userRepository.GetOne(u => u.Email.Equals(AzimuthIdentity.Current.UserCredential.Email)).Id;
                    user = _userRepository.Get(userId);


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

                }
                _unitOfWork.Commit();
            }
            _notificationService.CreateNotification(Notifications.LikedPlaylist, user, recentlyPlaylist: playlist);

        }

        public void AddCurrentUserAsFavorite(int playlistId)
        {
            User user = null;
            Playlist playlist;
            using (_unitOfWork)
            {
                playlist = _playlistRepository.Get(playlistId);
                if (playlist == null)
                {
                    throw new BadRequestException("Playlist with Id does not exist");
                }
                if (AzimuthIdentity.Current != null)
                {
                    var userId = AzimuthIdentity.Current.UserCredential.Id;
                    user = _userRepository.Get(userId);


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
                }
                _unitOfWork.Commit();
            }
            _notificationService.CreateNotification(Notifications.FavoritedPlaylist, user, recentlyPlaylist: playlist);
        }

        public void RemoveCurrentUserAsLiker(int playlistId)
        {
            User user = null;
            Playlist playlist;
            using (_unitOfWork)
            {
                playlist = _playlistRepository.Get(playlistId);
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

                    user = _userRepository.GetOne(u => u.Id == userId);

                }
                _unitOfWork.Commit();
            }
            _notificationService.CreateNotification(Notifications.UnlikedPlaylist, user, recentlyPlaylist: playlist);
        }

        public void RemoveCurrentUserAsFavorite(int playlistId)
        {
            User user = null;
            Playlist playlist;
            using (_unitOfWork)
            {
                playlist = _playlistRepository.Get(playlistId);
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

                    user = _userRepository.GetOne(u => u.Id == userId);
                }
                _unitOfWork.Commit();
            }
            _notificationService.CreateNotification(Notifications.UnfavoritedPlaylist, user, recentlyPlaylist: playlist);
        }

        public Task<bool> IsLiked(int id)
        {
            var dop = AzimuthIdentity.Current;
            return Task.Run(() =>
            {
                using (_unitOfWork)
                {
                    var playlist = _playlistRepository.Get(id);
                    if (playlist == null)
                    {
                        throw new BadRequestException("Playlist with Id does not exist");
                    }
                    if (dop != null)
                    {
                        var userId =
                            _userRepository.GetOne(u => u.Email.Equals(dop.UserCredential.Email)).Id;
                        var liker = _likerRepository.GetOne(pair => pair.Playlist.Id == id && pair.Liker.Id == userId);
                        return (liker != null) && liker.IsLiked;
                    }
                    return false;
                }
            });
        }

        public Task<bool> IsFavorite(int id)
        {
            var dop = AzimuthIdentity.Current;
            return Task.Run(() =>
            {
                using (_unitOfWork)
                {
                    var playlist = _playlistRepository.Get(id);
                    if (playlist == null)
                    {
                        throw new BadRequestException("Playlist with Id does not exist");
                    }
                    if (dop != null)
                    {
                        var userId =
                            _userRepository.GetOne(u => u.Email.Equals(dop.UserCredential.Email)).Id;
                        var liker = _likerRepository.GetOne(pair => pair.Playlist.Id == id && pair.Liker.Id == userId);
                        return (liker != null) && liker.IsFavorite;
                    }
                    return false;
                }
            });
        }
    }
}