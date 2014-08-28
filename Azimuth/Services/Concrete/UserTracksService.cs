using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel;
using System.Linq;
using System.Management.Instrumentation;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.DataProviders.Concrete;
using Azimuth.DataProviders.Interfaces;
using Azimuth.Infrastructure.Concrete;
using Azimuth.Infrastructure.Exceptions;
using Azimuth.Services.Interfaces;
using Azimuth.Shared.Dto;
using WebGrease.Css.Extensions;

namespace Azimuth.Services.Concrete
{
    public class UserTracksService : IUserTracksService
    {
        private ISocialNetworkApi _socialNetworkApi;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserRepository _userRepository;
        private readonly PlaylistRepository _playlistRepository;
        private readonly TrackRepository _trackRepository;
        private readonly PlaylistTrackRepository _playlistTrackRepository;

        public UserTracksService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            _userRepository = _unitOfWork.GetRepository<User>() as UserRepository;
            _playlistRepository = _unitOfWork.GetRepository<Playlist>() as PlaylistRepository;
            _trackRepository = _unitOfWork.GetRepository<Track>() as TrackRepository;
            _playlistTrackRepository = _unitOfWork.GetRepository<PlaylistTrack>() as PlaylistTrackRepository;
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
            return await Task.Run(() =>
            {
                using (_unitOfWork)
                {
                    var pt =
                        _playlistTrackRepository.Get(x => x.Identifier.Playlist.Id == id)
                            .OrderBy(o => o.TrackPosition)
                            .ToList();

                    ICollection<TracksDto> tracks = pt.Select(s => new TracksDto
                    {
                        Name = s.Identifier.Track.Name,
                        Duration = s.Identifier.Track.Duration,
                        Genre = s.Identifier.Track.Genre,
                        Url = s.Identifier.Track.Url,
                        Album = s.Identifier.Track.Album.Name,
                        Artist = s.Identifier.Track.Album.Artist.Name
                    }).ToList();

                    _unitOfWork.Commit();
                    return tracks;
                }
            });
        }

        public async Task<ICollection<TracksDto>> GetUserTracks()
        {
            return await Task.Run(() =>
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

                    _unitOfWork.Commit();
                    return tracks;
                }
            });
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
                {
                    throw new BadRequestException("Track with current Id doesn't exist");
                }

                var playlistNext = _playlistRepository.GetOne(pl => pl.Id == playlistId);
                if (playlistNext == null)
                {
                    throw new BadRequestException("Playlist with current Id doesn't exist");
                }

                var playlistPrevious = _playlistRepository.GetOne(pl => pl.Id == track.Playlists.First().Id);
                if (playlistPrevious == null)
                {
                    throw new BadRequestException("Playlist with current Id doesn't exist");
                }

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
                {
                    throw new InstanceNotFoundException("Playlist with specified id does not exist");                    
                }

                playlist.Tracks.Add(track);
                _unitOfWork.Commit();
            }
        }

        public async void SetPlaylist(PlaylistData playlistData, string provider, int index)
        {
            _socialNetworkApi = SocialNetworkApiFactory.GetSocialNetworkApi(provider);

            using (_unitOfWork)
            {
                //get checked tracks
                var socialNetworkData = GetSocialNetworkData(provider);
                var trackDatas = await _socialNetworkApi.GetSelectedTracks(socialNetworkData.ThirdPartId,
                    playlistData.TrackIds,
                    socialNetworkData.AccessToken);

                var playlist = _playlistRepository.GetOne(pl => pl.Id == playlistData.Id);
                playlist.PlaylistTracks = _playlistTrackRepository.Get(s => s.Identifier.Playlist.Id == playlistData.Id).ToList();

                int i = 0;
                //create Track objects
                var artistRepo = _unitOfWork.GetRepository<Artist>();
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
                        Duration = trackData.Duration.ToString(CultureInfo.InvariantCulture),
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
                    _trackRepository.AddItem(track);

                    if (index == -1)
                    {
                        index = (playlist.Tracks.Any()) ? playlist.Tracks.Count() : 0;
                    }
                    
                    var playlistTrack = new PlaylistTrack
                    {
                        Identifier = new PlaylistTracksIdentifier
                        {
                            Playlist = playlist,
                            Track = track
                        },
                        TrackPosition = index + i++
                    };

                    _playlistTrackRepository.AddItem(playlistTrack);

                    playlist.PlaylistTracks.Add(playlistTrack);
                }

                playlist.PlaylistTracks.ForEach((item, n) =>
                {
                    if (n < playlist.PlaylistTracks.Count - i && item.TrackPosition >= index)
                    {
                        item.TrackPosition += i;
                    }
                });

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