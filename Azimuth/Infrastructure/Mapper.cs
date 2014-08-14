
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
            AddMapp<VKUserData.Response, User>(VKUserDataMap);
            AddMapp<GoogleUserData, User>(GoogleUserDataMap);
            AddMapp<TweetSharp.TwitterUser, User>(TWitterUserDataMap);
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
                if (snData.location != null && snData.location.name != null)
                {
                    city = snData.location.name.Split(',').First();
                    country = snData.location.name.Split(' ').Last();
                }

                user.Name = new Name {FirstName = snData.first_name, LastName = snData.last_name};
                user.ScreenName = snData.name ?? String.Empty;
                user.Gender = snData.gender ?? String.Empty;
                user.Birthday = snData.birthday ?? String.Empty;
                user.Email = snData.email ?? String.Empty;
                user.Location =
                    new Location
                    {
                        City = city,
                        Country = country
                    };
                user.Timezone = snData.timezone;
                user.Photo = snData.picture.data.url;
        }

        private static void GoogleUserDataMap(GoogleUserData snData, User user)
        {
            string city = null;
            string country = null;
            var myPlace = ((snData.placesLived ?? new GoogleUserData.GoogleLocation[] { }).FirstOrDefault(p => p.primary) ?? new GoogleUserData.GoogleLocation { value = String.Empty }).value;
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
                FirstName = snData.name.givenName ?? String.Empty,
                LastName = snData.name.familyName ?? String.Empty
            };
            user.ScreenName = snData.displayName ?? String.Empty;
            user.Gender = snData.gender ?? String.Empty;
            user.Birthday = snData.birthday ?? String.Empty;
            user.Email = snData.emails.FirstOrDefault(e => e.type.Equals("account")).value ?? String.Empty;
            user.Location =
                new Location
                {
                    City = city ?? String.Empty,
                    Country = country ?? String.Empty
                };
            user.Timezone = -100;
            user.Photo = snData.image.url ?? String.Empty;
        }

        private static void VKUserDataMap(VKUserData.Response snData, User user)
        {
            user.Name =
                new Name
                {
                    FirstName = snData.response.First().first_name ?? String.Empty,
                    LastName = snData.response.First().last_name ?? String.Empty
                };
            user.ScreenName = snData.response.First().screen_name ?? String.Empty;
            user.Gender = (snData.response.First().sex != 0) ? snData.response.First().sex.ToString() : String.Empty;
            user.Birthday = snData.response.First().bdate ?? String.Empty;
            user.Email = snData.response.First().email ?? String.Empty;
            user.Location =
                new Location
                {
                    City = snData.response.First().city.title ?? String.Empty,
                    Country = snData.response.First().country.title ?? String.Empty
                };
            user.Timezone = snData.response.First().timezone;
            user.Photo = snData.response.First().photo_max_orig;
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
    }
}