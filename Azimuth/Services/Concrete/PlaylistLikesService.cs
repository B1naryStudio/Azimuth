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
    public class PlaylistLikesService : IPlaylistLikesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<PlaylistLike> _likerRepository;
        private readonly UserRepository _userRepository;
        private readonly IRepository<Playlist> _playlistRepository;

        public PlaylistLikesService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _likerRepository = _unitOfWork.GetRepository<PlaylistLike>();
            _userRepository = _unitOfWork.GetRepository<User>() as UserRepository;
            _playlistRepository = _unitOfWork.GetRepository<Playlist>();
        }

        public Task<List<User>> GetLikersByPlaylistId(int id)
        {
            return Task.Run(() =>
            {
                using (_unitOfWork)
                {
                    var listeners = _likerRepository.Get(list => list.Playlist.Id == id).Select(list=>list.Liker).ToList();
                    return listeners;
                }
                
            });
        }

        public void AddNewLike(int playlistId, int userId)
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
                    _likerRepository.AddItem(new PlaylistLike
                    {
                        Liker = user,
                        Playlist = playlist
                    });
                    _unitOfWork.Commit();
                }

            });
        }

        public void AddCurrentUserAsLiker(int playlistId)
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

                        
                    _likerRepository.AddItem(new PlaylistLike
                    {
                        Liker = user,
                        Playlist = playlist
                    });
                }
                _unitOfWork.Commit();
            }
        }

        public void RemoveCurrentUserAsLiker(int playlistId)
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
                    var listener = _likerRepository.GetOne(pair => pair.Playlist.Id == playlistId && pair.Liker.Id == userId);
                    _likerRepository.DeleteItem(listener);
                }
                _unitOfWork.Commit();
            }
        }

        public void RemoveLike(int playlistId, int userId)
        {
            Task.Run(() =>
            {
                using (_unitOfWork)
                {
                    var listener = _likerRepository.GetOne(pair => pair.Playlist.Id == playlistId && pair.Liker.Id == userId);
                    if (listener == null)
                    {
                        throw new BadRequestException("This listener pair does not exist");
                    }
                    _likerRepository.DeleteItem(listener);
                    _unitOfWork.Commit();
                }

            });
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
                        return liker != null;
                    }
                    return false;
                }
            });
        }
    }
}