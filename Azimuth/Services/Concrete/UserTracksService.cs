using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel;
using System.Linq;
using System.Management.Instrumentation;
using System.Text.RegularExpressions;
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
        private readonly LastfmApi _lastfmApi;
        private readonly DeezerApi _deezerApi;
        private readonly ChartLyricsApi _chartLyricsApi;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMusicServiceWorkUnit _musicServiceWorkUnit;
        private readonly UserRepository _userRepository;
        private readonly PlaylistRepository _playlistRepository;
        private readonly TrackRepository _trackRepository;
        private readonly PlaylistTrackRepository _playlistTrackRepository;

        public UserTracksService(IUnitOfWork unitOfWork, IMusicServiceWorkUnit musicServiceWorkUnit)
        {
            _unitOfWork = unitOfWork;

            _userRepository = _unitOfWork.GetRepository<User>() as UserRepository;
            _playlistRepository = _unitOfWork.GetRepository<Playlist>() as PlaylistRepository;
            _trackRepository = _unitOfWork.GetRepository<Track>() as TrackRepository;
            _playlistTrackRepository = _unitOfWork.GetRepository<PlaylistTrack>() as PlaylistTrackRepository;

            _musicServiceWorkUnit = musicServiceWorkUnit;
            _lastfmApi = _musicServiceWorkUnit.GetMusicService<LastfmTrackData>() as LastfmApi;
            _deezerApi = _musicServiceWorkUnit.GetMusicService<DeezerTrackData.TrackData>() as DeezerApi;
            _chartLyricsApi = _musicServiceWorkUnit.GetMusicService<string[]>() as ChartLyricsApi;
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

        public async Task<TrackInfoDto> GetTrackInfo(string artist, string trackName)
        {

            const string regExp = @"[^\w\d\s\/\\\.\,\[\]!@#$%^&*\(\);:?№']";

            int artistLength = (artist.Length > 50) ? 50 : artist.Length;
            int trackNameLength = (trackName.Length > 50) ? 50 : trackName.Length;

            artist = Regex.Replace(artist, regExp, "").Substring(0, artistLength);
            trackName = Regex.Replace(trackName, regExp, "").Substring(0, trackNameLength);

            var trackData = new TrackInfoDto();

            var lastfmData = await _lastfmApi.GetTrackInfo(artist, trackName);
            var deezerData = await _deezerApi.GetTrackInfo(artist, trackName);
            string[] lyricData;
            lyricData = await _chartLyricsApi.GetTrackInfo(artist, trackName);
            if (lyricData == null || !lyricData.Any() || (lyricData.Count() == 1 && String.IsNullOrEmpty(lyricData[0])))
            {
                _socialNetworkApi = SocialNetworkApiFactory.GetSocialNetworkApi("Vkontakte");
                UserSocialNetwork socialNetworkData = null;
                using (_unitOfWork)
                {
                    socialNetworkData = GetSocialNetworkData("Vkontakte");
                    _unitOfWork.Commit();
                }
                if (socialNetworkData != null)
                {
                    lyricData =
                        await
                            _socialNetworkApi.GetTrackLyricByArtistAndName(artist, trackName,
                                socialNetworkData.AccessToken, socialNetworkData.ThirdPartId);
                }
            }

            if (lastfmData.Track != null)
            {
                Mapper.Map(lastfmData, trackData);
            }
            if (deezerData != null)
            {
                Mapper.Map(deezerData, trackData);
            }
            if (lyricData != null)
            {
                Mapper.Map(lyricData, trackData);
            }

            return trackData;
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
                        Id = s.Identifier.Track.Id,
                        Name = s.Identifier.Track.Name,
                        Duration = s.Identifier.Track.Duration,
                        Album = s.Identifier.Track.Album.Name,
                        Artist = s.Identifier.Track.Album.Artist.Name,
                        ThirdPartId = s.Identifier.Track.ThirdPartId,
                        OwnerId = s.Identifier.Track.OwnerId
                    }).ToList();

                    _unitOfWork.Commit();
                    return tracks;
                }
            });
        }

        public ICollection<TracksDto> GetTracksByPlaylistIdSync(int id)
        {
            using (_unitOfWork)
            {
                var pt =
                    _playlistTrackRepository.Get(x => x.Identifier.Playlist.Id == id)
                        .OrderBy(o => o.TrackPosition)
                        .ToList();

                ICollection<TracksDto> tracks = pt.Select(s => new TracksDto
                {
                    Id = s.Identifier.Track.Id,
                    Name = s.Identifier.Track.Name,
                    Duration = s.Identifier.Track.Duration,
                    Album = s.Identifier.Track.Album.Name,
                    Artist = s.Identifier.Track.Album.Artist.Name,
                    ThirdPartId = s.Identifier.Track.ThirdPartId,
                    OwnerId = s.Identifier.Track.OwnerId
                }).ToList();

                _unitOfWork.Commit();
                return tracks;
            }
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
                            Album = track.Album.Name,
                            Artist = track.Album.Artist.Name,
                            ThirdPartId = track.ThirdPartId,
                            OwnerId = track.OwnerId
                        }).ToList();

                    _unitOfWork.Commit();
                    return tracks;
                }
            });
        }

        public async Task<List<string>> GetTrackUrl(TrackSocialInfo tracks, string provider)
        {
            string accessToken;
            if (AzimuthIdentity.Current == null)
            {
                var admin =
                    _userRepository.GetFullUserData(
                        _userRepository.Get(user => user.ScreenName == "id268940215"&& user.Name.FirstName == "Azimuth" && user.Name.LastName == "Azimuth")
                            .FirstOrDefault()
                            .Id);
                accessToken =
                    admin.SocialNetworks.FirstOrDefault(sn => sn.SocialNetwork.Name == "Vkontakte").AccessToken;
            }
            else
            {
                using (_unitOfWork)
                {
                    accessToken = GetSocialNetworkData(provider).AccessToken;
                    _unitOfWork.Commit();
                }
            }

            _socialNetworkApi = SocialNetworkApiFactory.GetSocialNetworkApi(provider);

            return await _socialNetworkApi.GetTrackUrl(tracks, accessToken);

        }

        public async Task<List<TracksDto>> VkontakteSearch(string searchText, int offset)
        {
            var trackDtos = new List<TracksDto>();
            var findInVk = new List<TrackData.Audio>();
            _socialNetworkApi = SocialNetworkApiFactory.GetSocialNetworkApi("Vkontakte");
            var socialNetworkData = GetSocialNetworkData("Vkontakte");
            findInVk = await _socialNetworkApi.SearchTracks(searchText, socialNetworkData.AccessToken, 0, offset, 20);
            if (findInVk.Count > 0)
            {
                findInVk.ForEach(item =>
                {
                    var dto = new TracksDto();
                    Mapper.Map(item, dto);
                    if (!trackDtos.Any(track => (track.Artist == dto.Artist) && (track.Name == dto.Name)))
                    {
                        trackDtos.Add(dto);
                    }
                });
            }
            return trackDtos;
        }

        public async Task<List<TracksDto>> MakeSearch(string searchText, string criteria)
        {
            var trackDtos = new List<TracksDto>();
            var tracks = new List<Track>();
            var findInVk = new List<TrackData.Audio>();
            using (_unitOfWork)
            {
                var socialNetworkData = GetSocialNetworkData("Vkontakte");
                _socialNetworkApi = SocialNetworkApiFactory.GetSocialNetworkApi("Vkontakte");

                switch (criteria)
                {
                    case "genre":
                        tracks = _trackRepository.Get(track => track.Genre.ToLower().Contains(searchText)).ToList();
                        break;
                    case "artist":
                        tracks =
                            _trackRepository.Get(track => track.Album.Artist.Name.ToLower().Contains(searchText))
                                .ToList();
                        break;
                    case "track":
                        tracks = _trackRepository.Get(track => track.Name.ToLower().Contains(searchText)).ToList();
                        break;
                    case "vkontakte":
                        findInVk =
                            await _socialNetworkApi.SearchTracks(searchText, socialNetworkData.AccessToken, 0, 0, 20);
                        if (findInVk.Count > 0)
                        {
                            findInVk.ForEach(item =>
                            {
                                var dto = new TracksDto();
                                Mapper.Map(item, dto);
                                if (!trackDtos.Any(track => (track.Artist == dto.Artist) && (track.Name == dto.Name)))
                                {
                                    trackDtos.Add(dto);
                                }
                            });
                        }
                        break;
                    case "myvktracks":
                        findInVk =
                            await
                                _socialNetworkApi.GetTracks(socialNetworkData.ThirdPartId, socialNetworkData.AccessToken);
                        findInVk =
                            findInVk.Where(
                                s =>
                                    s.Title.ToLower().Contains(searchText) ||
                                    s.GenreId.ToString().ToLower().Contains(searchText) ||
                                    s.Artist.ToLower().Contains(searchText)).ToList();

                        if (findInVk.Count > 0)
                        {
                            findInVk.ForEach(item =>
                            {
                                var dto = new TracksDto();
                                Mapper.Map(item, dto);
                                if (!trackDtos.Any(track => (track.Artist == dto.Artist) && (track.Name == dto.Name)))
                                {
                                    trackDtos.Add(dto);
                                }
                            });
                        }
                        break;
                }

                if (tracks.Count > 0)
                {
                    tracks.ForEach(item =>
                    {
                        var dto = new TracksDto();
                        Mapper.Map(item, dto);
                        if (!trackDtos.Any(track => (track.Artist == dto.Artist) && (track.Name == dto.Name)))
                        {
                            trackDtos.Add(dto);
                        }
                    });
                }

                _unitOfWork.Commit();
            }

            return trackDtos;
        }

        public void UpdateTrackPlaylistPosition(long playlistId, int newIndex, List<long> trackId)
        {
            using (_unitOfWork)
            {
                var pt = _playlistTrackRepository.Get(s => s.Identifier.Playlist.Id == playlistId).ToList();
                int i = 0;
                foreach (var id in trackId)
                {
                    int oldIndex = pt.Where(s => s.Identifier.Track.Id == id).Select(pos => pos.TrackPosition).First();
                    pt.Where(s => s.Identifier.Track.Id == id).Select((item) =>
                    {
                        item.TrackPosition = newIndex + i++;
                        return item;
                    }).ToList();


                    if (oldIndex > newIndex)
                    {
                        // +
                        pt.Where(
                            s =>
                                !trackId.Contains(s.Identifier.Track.Id) && s.TrackPosition >= newIndex &&
                                s.TrackPosition < oldIndex).Select(
                                    (item) =>
                                    {
                                        item.TrackPosition++;
                                        return item;
                                    }).ToList();
                    }
                    else
                    {
                        // -
                        pt.Where(
                            s =>
                                !trackId.Contains(s.Identifier.Track.Id) &&
                                s.TrackPosition <= newIndex + trackId.Count() - 1 &&
                                s.TrackPosition > oldIndex - i).Select(
                                    (item) =>
                                    {
                                        item.TrackPosition--;
                                        return item;
                                    }).ToList();
                    }
                }
                _unitOfWork.Commit();
            }
        }

        public void UpdateWholePlaylistTrackPositions(List<TrackInPlaylist> playlist, long playlistId)
        {
            using (_unitOfWork)
            {
                var pt = _playlistTrackRepository.Get(s => s.Identifier.Playlist.Id == playlistId).ToList();

                playlist.ForEach(item =>
                {
                    pt.Where(track => track.Identifier.Track.Id == item.TrackId)
                        .Select(pos => pos.TrackPosition = item.TrackPosition).ToList();
                });

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

        public async Task<List<TrackData.Audio>> SearchTracksInSn(List<TrackSearchInfo.SearchData> tracksDescription,
            string provider)
        {
            _socialNetworkApi = SocialNetworkApiFactory.GetSocialNetworkApi(provider);
            List<TrackData.Audio> searchedTracks = new List<TrackData.Audio>();

            using (_unitOfWork)
            {
                var socialNetworkData = GetSocialNetworkData(provider);
                searchedTracks =
                    await _socialNetworkApi.SearchTracksForLyric(tracksDescription, socialNetworkData.AccessToken);
                _unitOfWork.Commit();
            }

            return searchedTracks;
        }

        public async Task SetPlaylist(DataForTrackSaving tracksInfo, string provider, int index)
        {
            _socialNetworkApi = SocialNetworkApiFactory.GetSocialNetworkApi(provider);

            bool tracksToEnd = index == -1;

            using (_unitOfWork)
            {
                List<VkTrackResponse.Audio> trackDatas = null;
                //get checked tracks
                var socialNetworkData = GetSocialNetworkData(provider);
                trackDatas = await _socialNetworkApi.GetSelectedTracks(tracksInfo, socialNetworkData.AccessToken);

                if (trackDatas.Any())
                {
                    var playlist = _playlistRepository.GetOne(pl => pl.Id == tracksInfo.PlaylistId);
                    playlist.PlaylistTracks =
                        _playlistTrackRepository.Get(s => s.Identifier.Playlist.Id == tracksInfo.PlaylistId).ToList();

                    int i = 0;
                    //create Track objects
                    var artistRepo = _unitOfWork.GetRepository<Artist>();

                    try
                    {
                        foreach (var trackData in trackDatas)
                        {
                            if (trackData.Artist.Length > 50)
                            {
                                trackData.Artist = trackData.Artist.Substring(0, 50);
                            }
                            if (trackData.Title.Length > 50)
                            {
                                trackData.Title = trackData.Title.Substring(0, 50);
                            }

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
                                Album = album,
                                ThirdPartId = Convert.ToString(trackData.Id),
                                OwnerId = Convert.ToString(trackData.OwnerId),
                                Genre = trackData.GenreId.ToString(),
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
                    }
                    catch (Exception exp)
                    {
                        var s = exp;
                    }


                    if (tracksToEnd == false)
                    {
                        playlist.PlaylistTracks.ForEach((item, n) =>
                        {
                            if (n < playlist.PlaylistTracks.Count - i && item.TrackPosition >= index)
                            {
                                item.TrackPosition += i;
                            }
                        });
                    }
                }

                _unitOfWork.Commit();
            }
        }

        public async Task CopyTrackToAnotherPlaylist(long playlistId, List<long> trackIds)
        {
            using (_unitOfWork)
            {

                var playlist = _playlistRepository.GetOne(pl => pl.Id == playlistId);
                var tracks = _trackRepository.Get(tr => trackIds.Contains(tr.Id)).ToList();
                int i = 0;
                tracks.ForEach(track =>
                {
                    track.Playlists.Add(playlist);
                    var playlistTrack = new PlaylistTrack
                    {
                        Identifier = new PlaylistTracksIdentifier
                        {
                            Playlist = playlist,
                            Track = track,
                        },
                        TrackPosition = playlist.Tracks.Count() + i++
                    };
                    _playlistTrackRepository.AddItem(playlistTrack);
                    playlist.PlaylistTracks.Add(playlistTrack);
                });

                _unitOfWork.Commit();
            }
        }

        public async Task DeleteTracksFromPlaylist(long playlistId, List<long> trackIds)
        {
            using (_unitOfWork)
            {
                var playlist = _playlistRepository.GetOne(pl => pl.Id == playlistId);
                var tracks = _trackRepository.Get(tr => trackIds.Contains(tr.Id)).ToList();
                tracks.ForEach(track =>
                {
                    track.Playlists.Remove(playlist);
                    var playlistTrackToDelete =
                        _playlistTrackRepository.Get(
                            pt => trackIds.Contains(pt.Identifier.Track.Id) && pt.Identifier.Playlist.Id == playlistId);
                    var curPlaylistPt = _playlistTrackRepository.Get(pt => pt.Identifier.Playlist.Id == playlistId);
                    playlistTrackToDelete.ForEach(pt =>
                    {
                        var curTrackPos = pt.TrackPosition;
                        _playlistTrackRepository.DeleteItem(pt);
                        playlist.PlaylistTracks.Remove(pt);
                        curPlaylistPt.ForEach(item =>
                        {
                            if (item.TrackPosition > curTrackPos)
                            {
                                item.TrackPosition--;
                            }
                        });
                    });
                });

                _unitOfWork.Commit();
            }
        }

        private UserSocialNetwork GetSocialNetworkData(string provider)
        {
            var userSocialNetworkRepo = _unitOfWork.GetRepository<UserSocialNetwork>();
            if (AzimuthIdentity.Current != null)
            {
                return userSocialNetworkRepo.GetOne(
                    s =>
                        (s.SocialNetwork.Name == provider) &&
                        (s.User.Email == AzimuthIdentity.Current.UserCredential.Email));
                _unitOfWork.Commit();
            }
            else
            {
                return null;
            }
        }

    }
}