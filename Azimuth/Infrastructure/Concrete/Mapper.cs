using System;
using System.Collections.Generic;
using System.Linq;
using Azimuth.DataAccess.Entities;
using Azimuth.Shared.Dto;
using Azimuth.Shared.Enums;

namespace Azimuth.Infrastructure.Concrete
{
    public static class Mapper
    {
        private static readonly Dictionary<Tuple<Type, Type>, object> _maps = new Dictionary<Tuple<Type, Type>, object>();

        static Mapper()
        {
            AddMapp<FacebookUserData, User>(FacebookUserDataMap);
            AddMapp<VkUserData.VkResponse, User>(VkUserDataMap);
            AddMapp<GoogleUserData, User>(GoogleUserDataMap);
            AddMapp<TweetSharp.TwitterUser, User>(TwitterUserDataMap);
            AddMapp<Track, TracksDto>(TrackMap);
            AddMapp<User, UserDto>(UserMap);
            AddMapp<List<User>, List<UserDto>>(UserListMap);
            AddMapp<DeezerTrackData.TrackData, TrackInfoDto>(DeezerDataMap);
            AddMapp<LastfmTrackData, TrackInfoDto>(LastfmDataMap);
            AddMapp<string[], TrackInfoDto>(ChartLyricMap);
            AddMapp<TrackData.Audio, TracksDto>(VkTrackToTrackDto);
            AddMapp<Playlist, PlaylistData>(PlaylistMap);
            AddMapp<Notification, NotificationDto>(NotificationMap);
        }

        private static void NotificationMap(Notification notification, NotificationDto notificationDto)
        {

            notificationDto.UserFirstName = notification.User.Name.FirstName;
            notificationDto.UserLastName = notification.User.Name.LastName;
            notificationDto.UserId = notification.User.Id;
            notificationDto.NotificationType = notification.NotificationType;
            notificationDto.UserPhoto = notification.User.Photo;
            notificationDto.DateTime = notification.NotificationDate.ToString("U");
             if (notification.RecentlyUser != null)
            {
                notificationDto.RecentlyUserId = notification.RecentlyUser.Id;
                notificationDto.RecentlyUserFirstName = notification.RecentlyUser.Name.FirstName;
                notificationDto.RecentlyUserLastName = notification.RecentlyUser.Name.LastName;
            }
            if (notification.RecentlyPlaylist != null)
            {
                if (notification.RecentlyPlaylist.Accessibilty != Accessibilty.Private)
                {
                    notificationDto.RecentlyPlaylistId = notification.RecentlyPlaylist.Id;
                }
                notificationDto.RecentlyPlaylistName = notification.RecentlyPlaylist.Name;
            }
        }

        public static void AddMapp<TSource, TDestination>(Action<TSource, TDestination> map)
            where TSource : class
            where TDestination : class
        {
            _maps.Add(Tuple.Create(typeof(TSource), typeof(TDestination)), map);
        }

        public static TDestination Map<TSource, TDestination>(TSource source, TDestination destionation)
            where TSource: class 
            where TDestination: class 
        {
            var key = Tuple.Create(typeof (TSource), typeof (TDestination));
            var map = (Action<TSource, TDestination>) _maps[key];

            if (map == null)
            {
                throw new Exception(String.Format("No mapping defined for {0} => {1}", typeof (TSource).Name,
                    typeof(TDestination).Name));
            }

            map(source, destionation);
            return destionation;
        }

        private static void FacebookUserDataMap(FacebookUserData snData, User user)
        {
                string city = "", country = "";
                if (snData.Location != null && snData.Location.Name != null)
                {
                    city = snData.Location.Name.Split(',').First();
                    country = snData.Location.Name.Split(' ').Last();
                }

                user.Name = new Name {FirstName = snData.FirstName, LastName = snData.LastName};
                user.ScreenName = snData.Name ?? String.Empty;
                user.Gender = snData.Gender ?? String.Empty;
                user.Birthday = snData.Birthday ?? String.Empty;
                user.Email = snData.Email ?? String.Empty;
                user.Location =
                    new Location
                    {
                        City = city,
                        Country = country
                    };
                user.Timezone = snData.Timqzone;
                user.Photo = snData.Picture.Data.Url;
        }

        private static void GoogleUserDataMap(GoogleUserData snData, User user)
        {
            string city = null;
            string country = null;
            var myPlace = ((snData.PlacesLived ?? new GoogleUserData.GoogleLocation[] { }).FirstOrDefault(p => p.Primary) ?? new GoogleUserData.GoogleLocation { Value = String.Empty }).Value;
            var places = myPlace.Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (places.Length == 1)
            {
                city = places.First();
            }
            if (places.Length > 1)
            {
                city = places.First();
                country = places.Last();
            }
            user.Name = new Name
            {
                FirstName = snData.Name.GivenName ?? String.Empty,
                LastName = snData.Name.FamilyName ?? String.Empty
            };
            user.ScreenName = snData.DisplayName ?? String.Empty;
            user.Gender = snData.Gender ?? String.Empty;
            user.Birthday = snData.Birthday ?? String.Empty;
            
            var email = snData.Emails.FirstOrDefault(e => e.Type.Equals("account"));
            if (email != null)
                user.Email = email.Value ?? String.Empty;
            
            user.Location =
                new Location
                {
                    City = city ?? String.Empty,
                    Country = country ?? String.Empty
                };
            user.Timezone = -100;
            user.Photo = snData.Image.Url ?? String.Empty;
        }

        private static void VkUserDataMap(VkUserData.VkResponse snData, User user)
        {
            user.Name =
                new Name
                {
                    FirstName = snData.Response.First().FirstName ?? String.Empty,
                    LastName = snData.Response.First().LastName ?? String.Empty
                };
            user.ScreenName = snData.Response.First().ScreenName ?? String.Empty;
            user.Gender = (snData.Response.First().Sex != 0) ? snData.Response.First().Sex.ToString() : String.Empty;
            user.Birthday = snData.Response.First().Birthday ?? String.Empty;
            user.Email = snData.Response.First().Email ?? String.Empty;
            user.Location =
                new Location
                {
                    City = (snData.Response.First().City !=null) ? snData.Response.First().City.Title : String.Empty,
                    Country = (snData.Response.First().Country != null) ? snData.Response.First().Country.Title : String.Empty
                };
            user.Timezone = snData.Response.First().Timezone;
            user.Photo = snData.Response.First().Photo;
        }

        private static void TwitterUserDataMap(TweetSharp.TwitterUser snData, User user)
        {
            user.Name = new Name {FirstName = snData.Name ?? String.Empty, LastName = String.Empty};
                user.Birthday = String.Empty;
            user.Email = String.Empty;
            user.Gender = String.Empty;
            user.Location = new Location {City = snData.Location ?? String.Empty, Country = String.Empty};
            user.Timezone = -100;
            user.ScreenName = snData.ScreenName ?? String.Empty;
            user.Photo = snData.ProfileImageUrl ?? String.Empty;
        }

        private static void TrackMap(Track track, TracksDto tracksDto)
        {
            tracksDto.Name = track.Name;
            tracksDto.Album = track.Album.Name;
            tracksDto.Artist = track.Album.Artist.Name;
            tracksDto.OwnerId = track.OwnerId;
            tracksDto.ThirdPartId = track.ThirdPartId;
            tracksDto.Duration = track.Duration;
            tracksDto.Genre = track.Genre;
            tracksDto.ThirdPartId = track.ThirdPartId;
            tracksDto.OwnerId = track.OwnerId;
        }

        private static void UserMap(User user, UserDto userDto)
        {
            userDto.Id = user.Id.ToString();
            userDto.FirstName = user.Name.FirstName;
            userDto.LastName = user.Name.LastName;
            userDto.Photo = user.Photo;
            userDto.ScreenName = user.ScreenName;
            userDto.Gender = user.Gender;
            userDto.City = user.Location.City;
            userDto.Country = user.Location.Country;
            userDto.Email = user.Email;
            userDto.Birthdate = user.Birthday;
        }

        private static void DeezerDataMap(DeezerTrackData.TrackData deezerData, TrackInfoDto info)
        {
            if (deezerData.Album != null)
            {
                info.AlbumCover = deezerData.Album.Cover;
                info.AlbumFans = deezerData.Album.Fans;
                info.AlbumRank = deezerData.Album.Rating;
                info.AlbumRelease = deezerData.Album.ReleaseDate;
                info.AlbumTitle = info.AlbumTitle ?? deezerData.Album.Title;
                if (deezerData.Album.Genres != null)
                    deezerData.Album.Genres.Data.ForEach(genre => info.Genres.Add(genre.Name));
            }
            if (deezerData.Artist != null)
            {
                info.Artist = info.Artist ?? deezerData.Artist.Name;
                info.ArtistFans = deezerData.Artist.FansNumber;
                info.ArtistImage = deezerData.Artist.Picture;
                info.ArtistTopTracksUrl = deezerData.Artist.TrackListUrl; // TOP?
            }

            info.Title = info.Title ?? deezerData.Title;
            info.TrackRank = deezerData.Rank;
            info.TrackDeezerUrl = deezerData.Link;

            if (deezerData.TopTracks != null)
            {
                deezerData.TopTracks.ForEach(track => info.ArtistTopTrackList.Add(new TrackSearchInfo.SearchData { Artist = track.Artist.Name, Name = track.Title }));
                if ((deezerData.Album != null) && (deezerData.Album.Genres != null))
                {
                    deezerData.Album.Genres.Data.ForEach(genre => info.Genres.Add(genre.Name));
                }    
            }
            
        }

        private static void LastfmDataMap(LastfmTrackData lastfmTrackData, TrackInfoDto trackInfoDto)
        {
            trackInfoDto.Title = trackInfoDto.Title ?? lastfmTrackData.Track.Name;
            trackInfoDto.TrackLastfmUrl = lastfmTrackData.Track.Url;

            if (lastfmTrackData.Track.TrackArtist != null)
            {
                trackInfoDto.Artist = trackInfoDto.Artist ?? lastfmTrackData.Track.TrackArtist.Name;
                trackInfoDto.ArtistLastfmUrl = lastfmTrackData.Track.TrackArtist.Url;
            }

            if (lastfmTrackData.Track.TrackAlbum != null)
            {
                trackInfoDto.AlbumTitle = trackInfoDto.AlbumTitle ?? lastfmTrackData.Track.TrackAlbum.Title;
                trackInfoDto.AlbumLastfmUrl = lastfmTrackData.Track.TrackAlbum.Url;
            }

            if (lastfmTrackData.Track.TrackWiki != null)
            {
                trackInfoDto.Summary = lastfmTrackData.Track.TrackWiki.Summary;
                trackInfoDto.Description = lastfmTrackData.Track.TrackWiki.Content;
                trackInfoDto.Published = lastfmTrackData.Track.TrackWiki.Published;
            }
        }

        private static void ChartLyricMap(string[] lyric, TrackInfoDto trackInfoDto)
        {
            if (lyric.Any())
            {
                trackInfoDto.Lyric = lyric;
            }
        }

        private static void VkTrackToTrackDto(TrackData.Audio vk, TracksDto dto)
        {
            dto.Name = vk.Title;
            dto.OwnerId = vk.OwnerId.ToString();
            dto.ThirdPartId = vk.Id.ToString();
            dto.Genre = vk.GenreId.ToString();
            dto.Url = vk.Url;
            dto.Duration = vk.Duration.ToString();
            dto.Artist = vk.Artist;
        }
        private static void UserListMap(List<User> users, List<UserDto> usersDto)
        {
            usersDto.AddRange(users.Select(user => new UserDto
            {
                Id = user.Id.ToString(),
                Birthdate = user.Birthday,
                City = user.Location.City,
                Country = user.Location.Country,
                Email = user.Email,
                FirstName = user.Name.FirstName,
                LastName = user.Name.LastName,
                Gender = user.Gender,
                Photo = user.Photo,
                ScreenName = user.ScreenName
            }));
        }

        private static void PlaylistMap(Playlist playlist, PlaylistData playlistData)
        {
            var creator = playlist.Creator;
            playlistData.Id = playlist.Id;
            playlistData.Name = playlist.Name;
            playlistData.Duration = playlist.Tracks.Sum(x => int.Parse(x.Duration));
            playlistData.Genres = playlist.Tracks.Select(x => x.Genre)
                .GroupBy(x => x, (key, values) => new {Name = key, Count = values.Count()})
                .OrderByDescending(x => x.Count)
                .Where(x => x.Name.ToLower() != "other" && x.Name.ToLower() != "undefined")
                .Select(x => x.Name)
                .Take(5)
                .ToList();
            playlistData.Creator = new UserBrief
            {
                Name = creator.Name.FirstName + ' ' + creator.Name.LastName,
                Email = creator.Email
            };
            playlistData.ItemsCount = playlist.Tracks.Count;
            playlistData.PlaylistListened = playlist.Listened;
            playlistData.PlaylistLikes = playlist.PlaylistLikes.Count(s => s.IsLiked);
            playlistData.PlaylistFavourited = playlist.PlaylistLikes.Count(s => s.IsFavorite);
            playlistData.Accessibilty = playlist.Accessibilty;
        }

    }
}