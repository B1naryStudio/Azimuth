﻿using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.Linq;
using System.Management.Instrumentation;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.Infrastructure;
using Azimuth.Shared.Dto;
using Azimuth.Shared.Enums;

namespace Azimuth.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PlaylistRepository _playlistRepository;

        public PlaylistService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            _playlistRepository = _unitOfWork.GetRepository<Playlist>() as PlaylistRepository;
        }

        public async Task<List<PlaylistData>> GetPublicPlaylists()
        {
            var playlists = _playlistRepository.Get(list => list.Accessibilty == Accessibilty.Public).Select(playlist => new PlaylistData
            {
                Id = playlist.Id,
                Name = playlist.Name,
                Tracks = playlist.Tracks.Select(track => Mapper.Map(track, new TracksDto())).ToList()
            }).ToList();

            return playlists;
        }

        public void SetAccessibilty(int id, Accessibilty accessibilty)
        {
            if (!Enum.IsDefined(typeof (Accessibilty), accessibilty))
            {
                throw new BadRequestException("Accessibilty not correct");
            }
            var playlist = _playlistRepository.GetOne(s => s.Id == id);
            if (playlist == null)
            {
                throw new BadRequestException("playlist with specified id does not exist");
            }
            playlist.Accessibilty = accessibilty;

            _unitOfWork.Commit();
        }

        public void CreatePlaylist(string name, Accessibilty accessibilty)
        {
            if (!Enum.IsDefined(typeof(Accessibilty), accessibilty))
            {
                throw new BadRequestException("Accessibilty not correct");
            }

            using (_unitOfWork)
            {
                var playlistRepo = _unitOfWork.GetRepository<Playlist>();
                if (playlistRepo.GetOne(s => (s.Name == name) && 
                                             (s.Creator.Email == AzimuthIdentity.Current.UserCredential.Email)) != null)
                {
                    throw new BadRequestException("Playlist with this name already exists");
                }

                //get current user
                var userRepo = _unitOfWork.GetRepository<User>();
                var user = userRepo.GetOne(s => (s.Email == AzimuthIdentity.Current.UserCredential.Email));

                var playlist = new Playlist
                {
                    Accessibilty = accessibilty,
                    Name = name,
                    Creator = user
                };

                playlistRepo.AddItem(playlist);

                _unitOfWork.Commit();
            }
        }

        public async Task<PlaylistData> GetPlaylistById(int id)
        {
            using (_unitOfWork)
            {
                var playlist = _playlistRepository.GetOne(s => s.Id == id);
                if (playlist == null)
                {
                    throw new InstanceNotFoundException("playlist with specified id does not exist");
                }
                return new PlaylistData
                {
                    Id = playlist.Id,
                    Name = playlist.Name,
                    Tracks = playlist.Tracks.Select(track => Mapper.Map(track, new TracksDto())).ToList()
                };
            }
        }

        public async Task<List<PlaylistData>> GetUsersPlaylists()
        {
            using (_unitOfWork)
            {
                var userRepo = _unitOfWork.GetRepository<User>() as UserRepository;
                var userId = userRepo.GetOne(u => u.Email.Equals(AzimuthIdentity.Current.UserCredential.Email)).Id;
                //var playlists = _playlistRepository.GetByCreatorId(userId).Select(playlist => new PlaylistData
                //{
                //    Id = playlist.Id,
                //    Name = playlist.Name,
                //    Tracks = playlist.Tracks.Select(track => Mapper.Map(track, new TracksDto())).ToList(),
                //    Accessibilty = playlist.Accessibilty
                //}).ToList();

                var playlists = _playlistRepository.Get(s => s.Creator.Id == userId).Select(playlist => new PlaylistData
                {
                    Id = playlist.Id,
                    Name = playlist.Name,
                    Tracks = playlist.Tracks.Select(track => Mapper.Map(track, new TracksDto())).ToList(),
                    Accessibilty = playlist.Accessibilty
                }).ToList();
                return playlists;
            }
        }
    }
}