﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataProviders.Concrete;
using Azimuth.DataProviders.Interfaces;
using Azimuth.Infrastructure;
using Azimuth.Infrastructure.Exceptions;
using Azimuth.Shared.Dto;

namespace Azimuth.Services
{
    public class UserTracksService : IUserTracksService
    {
        private ISocialNetworkApi _socialNetworkApi;
        private readonly IUnitOfWork _unitOfWork;
        public UserTracksService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TrackData>> GetTracks(string provider)
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
                var trackDatas =await _socialNetworkApi.GetTracksById(socialNetworkData.ThirdPartId,
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
                        Lyrics =
                            await _socialNetworkApi.GetLyricsById(socialNetworkData.ThirdPartId, trackData.Id,
                                    socialNetworkData.AccessToken),
                        Name = trackData.Title,
                        Url = trackData.Url,
                        Genre = trackData.Genre,
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