
using System;
using System.Collections.Generic;
using System.Linq;
using Azimuth.DataAccess.Entities;
using Azimuth.Shared.Dto;

namespace Azimuth.Infrastructure
{
    public static class Mapper
    {
        private static  Dictionary<Tuple<Type, Type>, object> _maps = new Dictionary<Tuple<Type, Type>, object>();

        static Mapper()
        {
            AddMapp<FacebookUserData, User>(FacebookUserDataMap);
            AddMapp<VKUserData.VKResponse, User>(VKUserDataMap);
            AddMapp<GoogleUserData, User>(GoogleUserDataMap);
            AddMapp<TweetSharp.TwitterUser, User>(TWitterUserDataMap);
            AddMapp<Track, TracksDto>(TrackMap);
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
                throw new Exception(String.Format("No mapping defined for {0} => {1}", typeof (TSource).Name,
                    typeof (TDestination).Name));

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
            user.Email = snData.Emails.FirstOrDefault(e => e.Type.Equals("account")).Value ?? String.Empty;
            user.Location =
                new Location
                {
                    City = city ?? String.Empty,
                    Country = country ?? String.Empty
                };
            user.Timezone = -100;
            user.Photo = snData.Image.Url ?? String.Empty;
        }

        private static void VKUserDataMap(VKUserData.VKResponse snData, User user)
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
                    City = snData.Response.First().City.Title ?? String.Empty,
                    Country = snData.Response.First().Country.Title ?? String.Empty
                };
            user.Timezone = snData.Response.First().Timezone;
            user.Photo = snData.Response.First().Photo;
        }

        private static void TWitterUserDataMap(TweetSharp.TwitterUser snData, User user)
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
            tracksDto.Duration = track.Duration;
            tracksDto.Genre = track.Genre;
            tracksDto.Url = track.Url;
        }
    }
}