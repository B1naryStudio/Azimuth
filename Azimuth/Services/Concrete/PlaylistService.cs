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
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IMusicServiceWorkUnit _musicServiceWorkUnit;
        private readonly INotificationService _notificationService;
        private readonly LastfmApi _lastfmApi;

        public PlaylistService(IUnitOfWorkFactory unitOfWorkFactory, IMusicServiceWorkUnit musicServiceWorkUnit, INotificationService notificationService)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _musicServiceWorkUnit = musicServiceWorkUnit;
            _notificationService = notificationService;

            _lastfmApi = musicServiceWorkUnit.GetMusicService<LastfmTrackData>() as LastfmApi;
        }
 
        public async Task<List<PlaylistData>> GetPublicPlaylists()
        {
            return await Task.Run(() =>
            {
                using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
                {
                    var playlists = unitOfWork.PlaylistRepository.Get(list => list.Accessibilty == Accessibilty.Public).Select(playlist =>
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
                                UserId = creator.Id,
                                Name = creator.Name.FirstName + ' ' + creator.Name.LastName,
                                Email = creator.Email
                            },
                            ItemsCount = playlist.Tracks.Count,
                            PlaylistListened = playlist.Listened,
                            PlaylistLikes = playlist.PlaylistLikes.Count(s => s.IsLiked),
                            PlaylistFavourited = playlist.PlaylistLikes.Count(s => s.IsFavorite)
                        };
                    }).ToList();
                    unitOfWork.Commit();
                    return playlists;
                }
            });
        }

        public List<PlaylistData> GetPublicPlaylistsSync()
        {
            using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
            {
                long currentId = -1;
                if (AzimuthIdentity.Current != null)
                {
                    currentId = AzimuthIdentity.Current.UserCredential.Id;
                }
                var playlists = unitOfWork.PlaylistRepository
                    .Get(list => list.Accessibilty == Accessibilty.Public
                                 && list.Creator.Id != currentId)
                                 .Select(GetPlaylistData())
                                 .OrderByDescending(order => order.PlaylistListened)
                                 .ToList();
                unitOfWork.Commit();
                return playlists;
            }
        }

        public List<PlaylistData> GetPublicPlaylistsSync(long? id)
        {
            using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
            {
                var playlists = unitOfWork.PlaylistRepository
                    .Get(list => list.Accessibilty == Accessibilty.Public
                                 && list.Creator.Id == id)
                                 .Select(GetPlaylistData())
                                 .OrderByDescending(order => order.PlaylistListened)
                                 .ToList();
                unitOfWork.Commit();
                return playlists;
            }
        }

        public async Task<List<PlaylistData>> GetFavoritePlaylists()
        {
            return await Task.Run(() =>
            {
                List<PlaylistData> result = new List<PlaylistData>();
                using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
                {
                    var _likesRepository = unitOfWork.GetRepository<PlaylistLike>();
                    if (AzimuthIdentity.Current != null)
                    {
                        var userId = AzimuthIdentity.Current.UserCredential.Id;
                        var currentUser = unitOfWork.UserRepository.Get(userId);
                        var likedPlaylists = _likesRepository.Get(item => item.Liker.Id == currentUser.Id && item.IsFavorite).Select(item => item.Playlist).ToList();
                        result = unitOfWork.PlaylistRepository.Get(i => likedPlaylists.Any(j => i.Id == j.Id)).Select(playlist => Mapper.Map(playlist, new PlaylistData())).ToList();
                    }
                    unitOfWork.Commit();
                    return result;
                }
            });
        }

        public async Task<List<PlaylistData>> GetNotOwnedFavoritePlaylists()
        {
            var dop = AzimuthIdentity.Current;
            return await Task.Run(() =>
            {
                List<PlaylistData> result = new List<PlaylistData>();
                using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
                {
                    var _likesRepository = unitOfWork.GetRepository<PlaylistLike>();
                    if (dop != null)
                    {
                        var userId = dop.UserCredential.Id;
                        //_userRepository.GetOne(u => u.Email.Equals(dop.UserCredential.Email)).Id;
                        var currentUser = unitOfWork.UserRepository.Get(userId);
                        var likedPlaylists = _likesRepository.Get(item => item.Liker.Id == currentUser.Id && item.IsFavorite).Select(item => item.Playlist).ToList();
                        result = unitOfWork.PlaylistRepository.Get(i => (i.Creator.Id != currentUser.Id) &&
                            likedPlaylists.Any(j => i.Id == j.Id)).Select(playlist => Mapper.Map(playlist, new PlaylistData())).ToList();
                    }
                    unitOfWork.Commit();
                    return result;
                }
            });
        }

        public void SetAccessibilty(int id, Accessibilty accessibilty)
        {
            using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
            {
                if (!Enum.IsDefined(typeof(Accessibilty), accessibilty))
                {
                    throw new BadRequestException("Accessibilty not correct");
                }
                var playlist = unitOfWork.PlaylistRepository.GetOne(s => s.Id == id);
                if (playlist == null)
                {
                    throw new BadRequestException("playlist with specified id does not exist");
                }
                playlist.Accessibilty = accessibilty;

                var notification = _notificationService.CreateNotification(Notifications.ChangedPlaylistAccessebilty, playlist.Creator, recentlyPlaylist: playlist);

                playlist.Notifications.Add(notification);
                unitOfWork.NotificationRepository.AddItem(notification);

                unitOfWork.Commit();
            }
        }

        public long CreatePlaylist(string name, Accessibilty accessibilty)
        {
            long playlistId = -1;

            if (!Enum.IsDefined(typeof(Accessibilty), accessibilty))
            {
                throw new BadRequestException("Accessibilty not correct");
            }

            using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
            {
                var playlistRepo = unitOfWork.GetRepository<Playlist>();
                if (playlistRepo.GetOne(s => (s.Name == name) && 
                                             (s.Creator.Email == AzimuthIdentity.Current.UserCredential.Email)) != null)
                {
                    throw new BadRequestException("Playlist with this name already exists");
                }

                //get current user
                var userRepo = unitOfWork.GetRepository<User>();
                var user = userRepo.GetOne(s => (s.Email == AzimuthIdentity.Current.UserCredential.Email));

                var playlist = new Playlist
                {
                    Accessibilty = accessibilty,
                    Name = name,
                    Creator = user
                };

                playlistRepo.AddItem(playlist);
                playlistId = playlist.Id;

                var notification = _notificationService.CreateNotification(Notifications.PlaylistCreated, user, recentlyPlaylist: playlist);

                playlist.Notifications.Add(notification);
                unitOfWork.NotificationRepository.AddItem(notification);

                unitOfWork.Commit();
            }


            return playlistId;
        }

        public async Task<PlaylistData> GetPlaylistById(int id)
        {
            return await Task.Run(() =>
            {
                using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
                {
                    var playlist = unitOfWork.PlaylistRepository.GetOne(s => s.Id == id);
                    if (playlist == null)
                    {
                        throw new InstanceNotFoundException("playlist with specified id does not exist");
                    }

                    unitOfWork.Commit();
                    return Mapper.Map(playlist, new PlaylistData());
                }
            });
        }

        public async Task<List<PlaylistData>> GetUsersPlaylists()
        {
            long userId = -1;
            if (AzimuthIdentity.Current != null)
            {
                userId = AzimuthIdentity.Current.UserCredential.Id;
            }
            return await Task.Run(() =>
            {
                using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
                {
                    var playlists = unitOfWork.PlaylistRepository.Get(s => s.Creator.Id == userId).Select(playlist => Mapper.Map(playlist, new PlaylistData())).ToList();
                    unitOfWork.Commit();
                    return playlists;
                }
            });
        }

        public void RemovePlaylistById(int id)
        {
            using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
            {
                var _likesRepository = unitOfWork.GetRepository<PlaylistLike>();
                var playlist = unitOfWork.PlaylistRepository.GetOne(pl => pl.Id == id);
                
                if (playlist == null)
                {
                    throw new InstanceNotFoundException("Playlist with specified id does not exist");
                }

                if (AzimuthIdentity.Current != null)
                {
                    var userId = AzimuthIdentity.Current.UserCredential.Id;
                    if (userId == playlist.Creator.Id)
                    {
                        var playlistFollowing = _likesRepository.Get(pl => pl.Playlist.Id == playlist.Id && (pl.IsFavorite || pl.IsLiked)).Count();
                        if (playlistFollowing == 0)
                        {
                            unitOfWork.PlaylistRepository.DeleteItem(playlist);
                        }
                        else
                        {
                            var admin =
                                unitOfWork.UserRepository.GetOne(
                                    sysAdmin => sysAdmin.Name.FirstName.ToLower() == "azimuth" && sysAdmin.Name.LastName.ToLower() == "azimuth");
                            playlist.Creator = admin;
                        }
                    }
                    else
                    {
                        var favouritedPlaylist =
                            _likesRepository.GetOne(record => record.Liker.Id == userId && record.IsFavorite);
                        if (favouritedPlaylist != null)
                        {
                            _likesRepository.DeleteItem(favouritedPlaylist);
                        }
                    }
                }
                else
                {
                    throw new PrivilegeNotHeldException("Only creator can delete public playlist");
                }

                unitOfWork.Commit();
            }


        }

        public void RemoveTrackFromPlaylist(int trackId, int playlistId)
        {
            using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
            {
                var trackRepository = unitOfWork.GetRepository<Track>();
                var playlist = unitOfWork.PlaylistRepository.GetOne(pl => pl.Id == playlistId);
                if (playlist == null)
                {
                    throw new InstanceNotFoundException("Playlist with specified id does not exist");
                }

                var trackToDelete = trackRepository.GetOne(t => t.Id == trackId);

                if (trackToDelete == null)
                {
                    throw new InstanceNotFoundException("Track with specified id does not exist");
                }

                playlist.Tracks.Remove(trackToDelete);

                var notification = _notificationService.CreateNotification(Notifications.RemovedTracks, playlist.Creator, recentlyPlaylist: playlist);

                playlist.Notifications.Add(notification);
                unitOfWork.NotificationRepository.AddItem(notification);

                unitOfWork.Commit();
            }
        }

        public async Task<string> GetImageById(int id)
        {
            var image = String.Empty;

            using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
            {
                var playlist = unitOfWork.PlaylistRepository.GetOne(pl => pl.Id == id);

                if (playlist != null)
                {
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

                        seed = (seed < tracks.Count - 1) ? ++seed : 0;
                    }
                }

                unitOfWork.Commit();
            }

            return image;
        }

        public Task<string> GetSharedPlaylist(int playlistId)
        {
            return Task.Run(() =>
            {
                string guid;
                using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
                {
                    var sharedPlaylistRepository = unitOfWork.GetRepository<SharedPlaylist>();

                    var currentPlaylist = unitOfWork.PlaylistRepository.GetOne(p => p.Id == playlistId);
                    if (currentPlaylist == null)
                    {
                        return "";
                    }

                    var sysUser = unitOfWork.UserRepository
                        .GetOne(p => p.Name.FirstName == "Azimuth" && p.Name.LastName == "Azimuth");                                

                    guid = Guid.NewGuid().ToString();
                    var fakePlaylist = new Playlist
                    {
                        Name = "Share_" + guid,
                        Creator = sysUser,
                        Accessibilty = Accessibilty.Shared
                    };

                    foreach (var track in currentPlaylist.Tracks)
                    {
                        fakePlaylist.Tracks.Add(track);
                    }

                    unitOfWork.PlaylistRepository.AddItem(fakePlaylist);

                    var sharedPlaylist = new SharedPlaylist
                    {
                        Guid = guid,
                        Playlist = fakePlaylist
                    };
                    sharedPlaylistRepository.AddItem(sharedPlaylist);

                    var notification = _notificationService.CreateNotification(Notifications.PlaylistShared, currentPlaylist.Creator,
                        recentlyPlaylist: fakePlaylist);

                    fakePlaylist.Notifications.Add(notification);
                    unitOfWork.NotificationRepository.AddItem(notification);

                    unitOfWork.Commit();
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
            using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
            {
                var sharedPlaylistRepository = unitOfWork.GetRepository<SharedPlaylist>();
                var sharedPlaylist = sharedPlaylistRepository.GetOne(sp => sp.Guid == guid);

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
                
                unitOfWork.Commit();
            }

            return tracksDto;
        }

        public Task<string> GetSharedPlaylist(List<long> tracksId)
        {
            var currentUser = AzimuthIdentity.Current;

            return Task.Run(() =>
            {
                string guid;

                using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
                {
                    var trackRepository = unitOfWork.GetRepository<Track>();
                    var sharedPlaylistRepository = unitOfWork.GetRepository<SharedPlaylist>();

                    var tracks = trackRepository.Get(tr => tracksId.Contains(tr.Id)).ToList();
                    guid = Guid.NewGuid().ToString();

                    var sysUser = unitOfWork.UserRepository
                        .GetOne(p => p.Name.FirstName == "Azimuth" && p.Name.LastName == "Azimuth");

                    var fakePlaylist = new Playlist
                    {
                        Name = "Share_" + guid,
                        Creator = sysUser,
                        Accessibilty = Accessibilty.Shared
                    };

                    foreach (var track in tracks)
                    {
                        fakePlaylist.Tracks.Add(track);
                    }

                    unitOfWork.PlaylistRepository.AddItem(fakePlaylist);

                    var sharedPlaylist = new SharedPlaylist
                    {
                        Guid = guid,
                        Playlist = fakePlaylist
                    };

                    sharedPlaylistRepository.AddItem(sharedPlaylist);

                    var user = unitOfWork.UserRepository.GetOne(u => u.Id == currentUser.UserCredential.Id);

                    var notification = _notificationService.CreateNotification(Notifications.PlaylistShared, user,
                        recentlyPlaylist: fakePlaylist);

                    fakePlaylist.Notifications.Add(notification);
                    unitOfWork.NotificationRepository.AddItem(notification);
                    
                    unitOfWork.Commit();
                }
                
                return guid;
            });
        }

        public Task<string> SetPlaylistName(string azimuthPlaylist, string playlistName)
        {
            return Task.Run(() =>
            {
                using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
                {
                    var sharedPlaylistRepository = unitOfWork.GetRepository<SharedPlaylist>();
                    var guid = azimuthPlaylist;
                    var sharedPlaylist = sharedPlaylistRepository.GetOne(sp => sp.Guid == guid);

                    sharedPlaylist.Playlist.Name = playlistName;

                    unitOfWork.Commit();
                }

                return playlistName;
            });
        }

        public Task<int> RaiseListenedCount(int id)
        {
            return Task.Run(() =>
            {
                int listened = -1;
                using (var unitOfWork = _unitOfWorkFactory.NewUnitOfWork())
                {
                    var playlist = unitOfWork.PlaylistRepository.Get(item => item.Id == id).FirstOrDefault();
                    if (playlist != null)
                    {
                        listened = playlist.Listened++;
                    }
                    unitOfWork.Commit();
                    return listened;
                }

            });
        }

        private Func<Playlist, PlaylistData> GetPlaylistData()
        {
            return playlist =>
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
                        UserId = creator.Id,
                        Name = creator.Name.FirstName + ' ' + creator.Name.LastName,
                        Email = creator.Email
                    },

                    PlaylistListened = playlist.Listened,
                    PlaylistLikes = playlist.PlaylistLikes.Count(s => s.IsLiked),
                    PlaylistFavourited = playlist.PlaylistLikes.Count(s => s.IsFavorite),

                    ItemsCount = playlist.Tracks.Count,
                };
            };
        }
    }
}