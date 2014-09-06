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
using Azimuth.DataProviders.Concrete;
using Azimuth.DataProviders.Interfaces;
using Azimuth.Infrastructure.Concrete;
using Azimuth.Services.Interfaces;
using Azimuth.Shared.Dto;
using Azimuth.Shared.Enums;

namespace Azimuth.Services.Concrete
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMusicServiceWorkUnit _musicServiceWorkUnit;
        private readonly LastfmApi _lastfmApi;
        private readonly PlaylistRepository _playlistRepository;
        private readonly TrackRepository _trackRepository;
        private readonly UserRepository _userRepository;

        public PlaylistService(IUnitOfWork unitOfWork, IMusicServiceWorkUnit musicServiceWorkUnit)
        {
            _unitOfWork = unitOfWork;
            _musicServiceWorkUnit = musicServiceWorkUnit;

            _lastfmApi = musicServiceWorkUnit.GetMusicService<LastfmTrackData>() as LastfmApi;
            _playlistRepository = _unitOfWork.GetRepository<Playlist>() as PlaylistRepository;
            _trackRepository = _unitOfWork.GetRepository<Track>() as TrackRepository;
            _userRepository = _unitOfWork.GetRepository<User>() as UserRepository;
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
                                                .Where(x => x.Name.ToLower() != "other" && x.Name.ToLower() != "undefined")
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

        public long CreatePlaylist(string name, Accessibilty accessibilty)
        {
            long playlistId = -1;

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
                playlistId = playlist.Id;

                _unitOfWork.Commit();
            }
            return playlistId;
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
                                                .Where(x => x.Name.ToLower() != "other" && x.Name.ToLower() != "undefined")
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
                                                .Where(x => x.Name.ToLower() != "other" && x.Name.ToLower() != "undefined")
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

        public async Task<string> GetImageById(int id)
        {
            var image = String.Empty;

            using (_unitOfWork)
            {
                var playlist = _playlistRepository.GetOne(pl => pl.Id == id);

                var tracks = playlist.Tracks.ToList();

                if (!tracks.Any())
                {
                    return image;
                }

                var seed = new Random().Next(tracks.Count - 1);
                
                for (int i = 0; i < tracks.Count; i++)
                {
                    var artist = tracks[seed].Album.Artist.Name;
                    var trackName = tracks[seed].Name;

                    var trackInfoDto = await _lastfmApi.GetTrackInfo(artist, trackName);


                    if (trackInfoDto.Track != null 
                        && trackInfoDto.Track.TrackAlbum != null 
                        && trackInfoDto.Track.TrackAlbum.AlbumImages != null)
                    {
                        image = trackInfoDto.Track.TrackAlbum.AlbumImages.Last().Url;
                        if (image != String.Empty)
                        {
                            break;
                        }
                    }
    
                    seed = (seed < tracks.Count - 1) ? ++seed: 0;
                }

                _unitOfWork.Commit();
            }

            return image;
        }

        public Task<string> GetSharedPlaylist(int playlistId)
        {
            return Task.Run(() =>
            {
                string guid;
                using (_unitOfWork)
                {
                    var currentPlaylist = _playlistRepository.GetOne(p => p.Id == playlistId);
                    if (currentPlaylist == null)
                    {
                        return "";
                    }
                    
                    var sysUser = _userRepository.GetOne(p => p.Name.FirstName == "Admin" && p.Name.LastName == "Admin");

                    guid = Guid.NewGuid().ToString();
                    var fakePlaylist = new Playlist
                    {
                        Name = "Share_" + guid,
                        Creator = sysUser,
                        Accessibilty = Accessibilty.Public
                    };

                    foreach (var track in currentPlaylist.Tracks)
                    {
                        fakePlaylist.Tracks.Add(track);
                    }

                    _playlistRepository.AddItem(fakePlaylist);

                    var sharedRepo = _unitOfWork.GetRepository<SharedPlaylist>();
                    var sharedPlaylist = new SharedPlaylist
                    {
                        Guid = guid,
                        Playlist = fakePlaylist
                    };
                    sharedRepo.AddItem(sharedPlaylist);

                    _unitOfWork.Commit();
                }
                return guid;
            });
        }

        public List<TracksDto> GetSharedTracks(string guid)
        {
            if(string.IsNullOrEmpty(guid))
            {
                return null;
            }

            var tracksDto = new List<TracksDto>();
            using (_unitOfWork)
            {
                var sharedPlaylistRepo = _unitOfWork.GetRepository<SharedPlaylist>();

                var sharedPlaylist = sharedPlaylistRepo.GetOne(sp => sp.Guid == guid);

                if (sharedPlaylist != null && sharedPlaylist.Playlist != null)
                {
                    var tracks = sharedPlaylist.Playlist.Tracks;

                    foreach (var track in tracks)
                    {
                        var trackDto = new TracksDto();
                        Mapper.Map(track, trackDto);

                        tracksDto.Add(trackDto);
                    }    
                }
                
                _unitOfWork.Commit();
            }

            return tracksDto;
        }

        public Task<string> GetSharedPlaylist(List<long> tracksId)
        {
            return Task.Run(() =>
            {
                string guid;

                using (_unitOfWork)
                {
                    var tracks = _trackRepository.Get(tr => tracksId.Contains(tr.Id)).ToList();
                    guid = Guid.NewGuid().ToString();

                    var sysUser = _userRepository.GetOne(p => p.Name.FirstName == "Admin" && p.Name.LastName == "Admin");

                    var fakePlaylist = new Playlist
                    {
                        Name = "Share_" + guid,
                        Creator = sysUser,
                        Accessibilty = Accessibilty.Public
                    };

                    foreach (var track in tracks)
                    {
                        fakePlaylist.Tracks.Add(track);
                    }

                    _playlistRepository.AddItem(fakePlaylist);

                    var sharedRepo = _unitOfWork.GetRepository<SharedPlaylist>();
                    var sharedPlaylist = new SharedPlaylist
                    {
                        Guid = guid,
                        Playlist = fakePlaylist
                    };

                    sharedRepo.AddItem(sharedPlaylist);

                    _unitOfWork.Commit();
                }

                return guid;
            });
        }
    }
}