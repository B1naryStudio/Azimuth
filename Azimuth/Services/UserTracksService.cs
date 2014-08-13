using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.DataProviders.Concrete;
using Azimuth.DataProviders.Interfaces;
using Azimuth.Infrastructure;
using Azimuth.Shared.Dto;
using Azimuth.ViewModels;

namespace Azimuth.Services
{
    public class UserTracksService : IUserTracksService
    {
        private ISocialNetworkApi _socialNetworkApi;
        private readonly IUnitOfWork _unitOfWork;
        private IMusicService _musicService;
        private UserRepository _userRepository;
        private PlaylistRepository _playlistRepository;

        public UserTracksService(IUnitOfWork unitOfWork, IMusicService musicService)
        {
            _unitOfWork = unitOfWork;

            _musicService = musicService;
            _userRepository = _unitOfWork.GetRepository<User>() as UserRepository;
            _playlistRepository = _unitOfWork.GetRepository<Playlist>() as PlaylistRepository;
        }

        public async Task<List<TrackData>> GetTracks(string provider)
        {
            _socialNetworkApi = SocialNetworkApiFactory.GetSocialNetworkApi(provider);
            UserSocialNetwork user;

            using (_unitOfWork)
            {
                var userSocialNetworkRepo = _unitOfWork.GetRepository<UserSocialNetwork>();
                user = userSocialNetworkRepo.GetOne(
                    s =>
                        (s.SocialNetwork.Name == provider) &&
                        (s.User.Email == AzimuthIdentity.Current.UserCredential.Email));

                _unitOfWork.Commit();
            }

            return await _socialNetworkApi.GetTracks(user.ThirdPartId, user.AccessToken);
        }

        public async Task<ICollection<TracksDto>> GetUserTracks()
        {
            using (_unitOfWork)
            {
                var user = _userRepository.GetOne(s => s.Email == AzimuthIdentity.Current.UserCredential.Email);
                var playlists = _playlistRepository.Get(s => s.Creator.Id == user.Id).ToList();
                ICollection<TracksDto> tracks = new Collection<TracksDto>();

                tracks = playlists[0].Tracks.Select(track => new TracksDto
                {
                    Name = track.Name,
                    Duration = track.Duration,
                    Genre = track.Genre,
                    Url = track.Url,
                    Album = track.Album.Name,
                    Artist = track.Album.Artist.Name
                }).ToList();
                return tracks;
            }
        }
    }
}