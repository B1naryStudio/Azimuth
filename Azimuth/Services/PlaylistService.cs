﻿
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
using NHibernate;

namespace Azimuth.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private PlaylistRepository _playlistRepository;

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
    }
}