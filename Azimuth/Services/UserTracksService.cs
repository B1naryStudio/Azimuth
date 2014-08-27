using System.Collections.Generic;
using System.IdentityModel;
using System.Linq;
using System.Management.Instrumentation;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.DataProviders.Concrete;
using Azimuth.DataProviders.Interfaces;
using Azimuth.Infrastructure;
using Azimuth.Infrastructure.Exceptions;
using Azimuth.Shared.Dto;
using NHibernate.Util;

namespace Azimuth.Services
{
    public class UserTracksService : IUserTracksService
    {
        private ISocialNetworkApi _socialNetworkApi;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserRepository _userRepository;
        private readonly PlaylistRepository _playlistRepository;
        private readonly TrackRepository _trackRepository;

        public UserTracksService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            _userRepository = _unitOfWork.GetRepository<User>() as UserRepository;
            _playlistRepository = _unitOfWork.GetRepository<Playlist>() as PlaylistRepository;
            _trackRepository = _unitOfWork.GetRepository<Track>() as TrackRepository;
        }

        public async Task<List<TrackData.Audio>> GetTracks(string provider)
        {
            _socialNetworkApi = SocialNetworkApiFactory.GetSocialNetworkApi(provider);
            UserSocialNetwork socialNetworkData;

            using (_unitOfWork)
            {
                socialNetworkData = GetSocialNetworkData(provider);

                _unitOfWork.Commit();
            }

            if (socialNetworkData == null)
            {
                throw new EndUserException("Can't get social network info with provider name = " + provider);
            }

            return await _socialNetworkApi.GetTracks(socialNetworkData.ThirdPartId, socialNetworkData.AccessToken);
        }

        public async Task<ICollection<TracksDto>> GetTracksByPlaylistId(int id)
        {
            using (_unitOfWork)
            {
                var playlist = _playlistRepository.GetOne(x => x.Id == id);
                ICollection<TracksDto> tracks = playlist.Tracks.Select(track => new TracksDto
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

        public async Task<ICollection<TracksDto>> GetUserTracks()
        {
            using (_unitOfWork)
            {
                var user = _userRepository.GetOne(s => s.Email == AzimuthIdentity.Current.UserCredential.Email);
                var playlists = _playlistRepository.Get(s => s.Creator.Id == user.Id).ToList();
                ICollection<TracksDto> tracks =
                    playlists.SelectMany(s => s.Tracks).Distinct().Select(track => new TracksDto
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

        public void PutTrackToPlaylist(long playlistId, long trackId)
        {
            using (_unitOfWork)
            {
                var track = _trackRepository.GetOne(t => t.Id == trackId);
                if (track == null)
                    throw new BadRequestException("There is no track with current Id in database");

                var playlist = _playlistRepository.GetOne(pl => pl.Id == playlistId);
                if (playlist == null)
                    throw new BadRequestException("There is no playlist with current Id in database");                

                track.Playlists.Add(playlist);

                _unitOfWork.Commit();
            }
        }

        public void MoveTrackBetweenPlaylists(long playlistId, long trackId)
        {
            using (_unitOfWork)
            {
                var track = _trackRepository.GetOne(tr => tr.Id == trackId);
                if (track == null)
                    throw new BadRequestException("Track with current Id doesn't exist");

                var playlistNext = _playlistRepository.GetOne(pl => pl.Id == playlistId);
                if (playlistNext == null)
                    throw new BadRequestException("Playlist with current Id doesn't exist");

                var playlistPrevious = _playlistRepository.GetOne(pl => pl.Id == track.Playlists.First().Id);
                if (playlistPrevious == null)
                    throw new BadRequestException("Playlist with current Id doesn't exist");

                track.Playlists.Remove(playlistPrevious);
                track.Playlists.Add(playlistNext);

                _unitOfWork.Commit();
            }
        }

        public void PutTrackToPlaylist(int id, Track track)
        {
            using (_unitOfWork)
            {
                var playlist = _playlistRepository.GetOne(pl => pl.Id == id);
                if (playlist == null)
                    throw new InstanceNotFoundException("Playlist with specified id does not exist");

                playlist.Tracks.Add(track);
            }
        }

        public async void SetPlaylist(PlaylistData playlistData, string provider)
        {
            _socialNetworkApi = SocialNetworkApiFactory.GetSocialNetworkApi(provider);

            using (_unitOfWork)
            {
                //get current user
                var userRepo = _unitOfWork.GetRepository<User>();
                var user = userRepo.GetOne(s => (s.Email == AzimuthIdentity.Current.UserCredential.Email));

                //get checked tracks
                var socialNetworkData = GetSocialNetworkData(provider);
                var trackDatas =await _socialNetworkApi.GetSelectedTracks(socialNetworkData.ThirdPartId,
                    playlistData.TrackIds,
                    socialNetworkData.AccessToken);

                //set playlist
                var playlistRepo = _unitOfWork.GetRepository<Playlist>();
                var playlist = new Playlist
                {
                    Accessibilty = playlistData.Accessibilty,
                    Creator = user,
                    Name = playlistData.Name
                };

                //create Track objects
                var artistRepo = _unitOfWork.GetRepository<Artist>();
                var tracks = new List<Track>();
                foreach (var trackData in trackDatas)
                {
                    var artist = new Artist
                    {
                        Name = trackData.Artist
                    };
                    var album = new Album
                    {
                        Name = "",
                        Artist = artist,
                    };
                    artist.Albums.Add(album);
                    artistRepo.AddItem(artist);
                    var track = new Track
                    {
                        Duration = trackData.Duration.ToString(),
                        //Lyrics =
                        //    await _socialNetworkApi.GetLyricsById(socialNetworkData.ThirdPartId, trackData.Id,
                        //            socialNetworkData.AccessToken),
                        Name = trackData.Title,
                        Url = trackData.Url,
                        Genre = trackData.GenreId.ToString(),
                        Album = album
                    };
                    track.Playlists.Add(playlist);
                    album.Tracks.Add(track);
                    tracks.Add(track);
                }
                playlist.Tracks = tracks;
                
                playlistRepo.AddItem(playlist);

                _unitOfWork.Commit();
            }
        }

        private UserSocialNetwork GetSocialNetworkData(string provider)
        {
            var userSocialNetworkRepo = _unitOfWork.GetRepository<UserSocialNetwork>();
            return userSocialNetworkRepo.GetOne(
                s =>
                    (s.SocialNetwork.Name == provider) &&
                    (s.User.Email == AzimuthIdentity.Current.UserCredential.Email));
        }
    }
}