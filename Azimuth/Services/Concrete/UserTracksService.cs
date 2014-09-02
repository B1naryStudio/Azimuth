﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel;
using System.Linq;
using System.Management.Instrumentation;
using System.Threading.Tasks;
using System.Web.WebPages;
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
using CollectionExtensions = Castle.Core.Internal.CollectionExtensions;

namespace Azimuth.Services.Concrete
{
    public class UserTracksService : IUserTracksService
    {
        private ISocialNetworkApi _socialNetworkApi;
        private readonly IMusicService _lastfmApi;
        private readonly IDeezerService _deezerApi;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserRepository _userRepository;
        private readonly PlaylistRepository _playlistRepository;
        private readonly TrackRepository _trackRepository;
        private readonly PlaylistTrackRepository _playlistTrackRepository;

        public UserTracksService(IUnitOfWork unitOfWork, IMusicService lastfmApi, IDeezerService deezerApi)
        {
            _unitOfWork = unitOfWork;

            _lastfmApi = lastfmApi;
            _deezerApi = deezerApi;
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

        public async Task<TrackInfoDto> GetTrackInfo(string author, string trackName)
        {
            return await _lastfmApi.GetTrackInfo(author, trackName);
        }

        public async Task<DeezerTrackData> GetDeezerTrackInfo(string author, string trackName)
        {
            return await _deezerApi.GetTrackInfo(author, trackName);
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
                    //var negativeTest = pt.OrderByDescending(s=> s.TrackPosition < 0).Where(s => s.TrackPosition < 0).ToList();
                    //if (negativeTest.Count > 0)
                    //{
                    //    var neg = 0 - negativeTest[0].TrackPosition;
                    //    pt.Select((item) =>
                    //    {
                    //        item.TrackPosition += neg;
                    //        return item;
                    //    }).ToList();
                    //}
                }
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

        public async Task SetPlaylist(PlaylistData playlistData, string provider, int index, string friendId)
        {
            _socialNetworkApi = SocialNetworkApiFactory.GetSocialNetworkApi(provider);

            bool tracksToEnd = index == -1;

            using (_unitOfWork)
            {
                List<TrackData.Audio> trackDatas = null;
                //get checked tracks
                var socialNetworkData = GetSocialNetworkData(provider);
                if (!String.IsNullOrEmpty(friendId) && !String.IsNullOrWhiteSpace(friendId))
                {
                    trackDatas = await _socialNetworkApi.GetSelectedTracks(friendId,
                    playlistData.TrackIds,
                    socialNetworkData.AccessToken);
                }
                else
                {
                    trackDatas = await _socialNetworkApi.GetSelectedTracks(socialNetworkData.ThirdPartId,
                    playlistData.TrackIds,
                    socialNetworkData.AccessToken);
                }

                if (trackDatas.Any())
                {
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

        public async Task CopyTrackToAnotherPlaylist (long playlistId, List<long> trackIds )
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
                    var playlistTrackToDelete = _playlistTrackRepository.Get(pt => trackIds.Contains(pt.Identifier.Track.Id) && pt.Identifier.Playlist.Id == playlistId);
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
            return userSocialNetworkRepo.GetOne(
                s =>
                    (s.SocialNetwork.Name == provider) &&
                    (s.User.Email == AzimuthIdentity.Current.UserCredential.Email));
        }
    }
}