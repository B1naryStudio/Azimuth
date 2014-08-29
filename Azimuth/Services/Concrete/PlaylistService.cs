using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.Linq;
using System.Management.Instrumentation;
using System.Security.AccessControl;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.Infrastructure.Concrete;
using Azimuth.Services.Interfaces;
using Azimuth.Shared.Dto;
using Azimuth.Shared.Enums;

namespace Azimuth.Services.Concrete
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PlaylistRepository _playlistRepository;
        private readonly TrackRepository _trackRepository;

        public PlaylistService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            _playlistRepository = _unitOfWork.GetRepository<Playlist>() as PlaylistRepository;
            _trackRepository = _unitOfWork.GetRepository<Track>() as TrackRepository;
        }

        public async Task<List<PlaylistData>> GetPublicPlaylists()
        {
            return await Task.Run(() =>
            {
                var playlists = _playlistRepository.Get(list => list.Accessibilty == Accessibilty.Public).Select(playlist =>
                {
                    var creator = playlist.Creator;
                    return new PlaylistData
                    {
                        Id = playlist.Id,
                        Name = playlist.Name,
                        Duration = playlist.Tracks.Sum(x => int.Parse(x.Duration)),
                        Genres = playlist.Tracks.Select(x => x.Genre)
                            .GroupBy(x => x, (key, values) => new { Name = key, Count = values.Count() })
                            .OrderByDescending(x => x.Count)
                            .Select(x => x.Name)
                            .Take(5)
                            .ToList(),
                        Creator = new UserBrief
                        {
                            Name = creator.Name.FirstName + ' ' + creator.Name.LastName,
                            Email = creator.Email
                        },
                        ItemsCount = playlist.Tracks.Count,
                    };
                }).ToList();

                return playlists;
            });
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
            return await Task.Run(() =>
            {
                using (_unitOfWork)
                {
                    var playlist = _playlistRepository.GetOne(s => s.Id == id);
                    if (playlist == null)
                    {
                        throw new InstanceNotFoundException("playlist with specified id does not exist");
                    }

                    _unitOfWork.Commit();

                    return new PlaylistData
                    {
                        Id = playlist.Id,
                        Name = playlist.Name,
                        Accessibilty = playlist.Accessibilty,
                        Duration = playlist.Tracks.Sum(x => int.Parse(x.Duration)),
                        Genres = playlist.Tracks.Select(x => x.Genre)
                            .GroupBy(x => x, (key, values) => new {Name = key, Count = values.Count()})
                            .OrderByDescending(x => x.Count)
                            .Select(x => x.Name)
                            .Take(5)
                            .ToList(),
                        ItemsCount = playlist.Tracks.Count,
                    };
                }
            });
        }

        public async Task<List<PlaylistData>> GetUsersPlaylists()
        {
            var currentEmail = AzimuthIdentity.Current.UserCredential.Email;
            return await Task.Run(() =>
            {
                using (_unitOfWork)
                {
                    var userRepo = _unitOfWork.GetRepository<User>() as UserRepository;
                    if (userRepo == null)
                    {
                        throw new NullReferenceException();
                    }
                    var userId = userRepo.GetOne(u => u.Email.Equals(currentEmail)).Id;

                    var playlists = _playlistRepository.Get(s => s.Creator.Id == userId).Select(playlist => new PlaylistData
                    {
                        Id = playlist.Id,
                        Name = playlist.Name,
                        Duration = playlist.Tracks.Sum(x => int.Parse(x.Duration)),
                        Genres = playlist.Tracks.Select(x => x.Genre)
                                                .GroupBy(x => x, (key, values) => new { Name = key, Count = values.Count() })
                                                .OrderByDescending(x => x.Count)
                                                .Select(x => x.Name)
                                                .Take(5)
                                                .ToList(),
                        ItemsCount = playlist.Tracks.Count,
                        Accessibilty = playlist.Accessibilty
                    }).ToList();
                    return playlists;
                }
            });
                
        }

        public void RemovePlaylistById(int id)
        {
            using (_unitOfWork)
            {
                var playlist = _playlistRepository.GetOne(pl => pl.Id == id);
                
                if (playlist == null)
                {
                    throw new InstanceNotFoundException("Playlist with specified id does not exist");
                }
                var userRepo = _unitOfWork.GetRepository<User>() as UserRepository;
                if (userRepo == null)
                {
                    throw new NullReferenceException();
                }

                if (AzimuthIdentity.Current != null)
                {
                    var userId = userRepo.GetOne(user => user.Email.Equals(AzimuthIdentity.Current.UserCredential.Email)).Id;
                    if (userId == playlist.Creator.Id)
                    {
                        _playlistRepository.DeleteItem(playlist);
                    }
                    else
                    {
                        throw new PrivilegeNotHeldException("Only creator can delete public playlist");
                    }
                }
                else
                {
                    throw new PrivilegeNotHeldException("Only creator can delete public playlist");
                }
            }
        }

        public void RemoveTrackFromPlaylist(int trackId, int playlistId)
        {
            using (_unitOfWork)
            {
                var playlist = _playlistRepository.GetOne(pl => pl.Id == playlistId);
                if (playlist == null)
                {
                    throw new InstanceNotFoundException("Playlist with specified id does not exist");
                }

                var trackToDelete = _trackRepository.GetOne(t => t.Id == trackId);

                if (trackToDelete == null)
                {
                    throw new InstanceNotFoundException("Track with specified id does not exist");
                }

                playlist.Tracks.Remove(trackToDelete);
                _unitOfWork.Commit();
            }
        }
    }
}