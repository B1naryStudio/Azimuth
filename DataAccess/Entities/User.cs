using System;
using System.Linq;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.Shared.Dto;
using TweetSharp;

namespace Azimuth.DataAccess.Entities
{
    public class User : BaseEntity
    {
        public virtual Name Name { get; set; }
        public virtual string ScreenName { get; set; }
        public virtual string Gender { get; set; }
        public virtual string Birthday { get; set; }
        public virtual string Photo { get; set; }
        public virtual int Timezone { get; set; }
        public virtual Location Location { get; set; }
        public virtual string Email { get; set; }
        public virtual Iesi.Collections.Generic.ISet<UserSocialNetwork> SocialNetworks { get; set; }

        public override string ToString()
        {
            return Name.FirstName + Name.LastName + ScreenName + Gender + Email + Birthday + Timezone + Location.City +
                   ", " + Location.Country + Photo; // Think about location format
        }

        /// <summary>
        /// Coverts GoogleUserData to User. Don't forget to assign timeZone after that
        /// </summary>
        /// <param name="googleUserData"></param>
        /// <returns></returns>
        public static explicit operator User(GoogleUserData userData)
        {
            string city = null;
            string country = null;
            var myPlace = ((userData.placesLived ?? new GoogleLocation[] { }).FirstOrDefault(p => p.primary) ?? new GoogleLocation { value = String.Empty }).value;
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
            return new User
            {
                Name = new Name { FirstName = userData.name.givenName?? String.Empty, LastName = userData.name.familyName ?? String.Empty },
                ScreenName = userData.displayName ?? String.Empty,
                Gender = userData.gender,
                Birthday = userData.birthday ?? String.Empty,
                Email = userData.emails.FirstOrDefault(e=>e.type.Equals("account")).value ?? String.Empty,
                Location =
                    new Location
                    {
                        City = city ?? String.Empty,
                        Country = country ?? String.Empty
                    },
                Timezone = -100,
                Photo = userData.image.url
            };
        }

        public static explicit operator User(FacebookUserData userData)
        {
            string city = "", country = "";

            if (userData.Location != null)
            {
                if (userData.Location.name != null)
                {
                    city = userData.Location.name.Split(',')[0];
                    country = userData.Location.name.Split(' ')[1];
                }
            }

            return new User
            {
                Name = new Name { FirstName = userData.first_name ?? String.Empty, LastName = userData.last_name ?? String.Empty },
                ScreenName = userData.name ?? String.Empty,
                Gender = userData.gender ?? String.Empty,
                Birthday = userData.birthday ?? String.Empty,
                Email = userData.email ?? String.Empty,
                Location =
                    new Location
                    {
                        City = city,
                        Country = country
                    },
                Timezone = userData.timezone,
                Photo = userData.Photo.url
            };
        }

        public static explicit operator User(VKUserdata userData)
        {
            return new User
            {
                Name =
                    new Name
                    {
                        FirstName = userData.first_name ?? String.Empty,
                        LastName = userData.last_name ?? String.Empty
                    },
                ScreenName = userData.screen_name ?? String.Empty,
                Gender = (userData.sex != 0) ? userData.sex.ToString() : String.Empty,
                Birthday = userData.bdate ?? String.Empty,
                Email = userData.email ?? String.Empty,
                Location =
                    new Location
                    {
                        City = userData.City.title ?? String.Empty,
                        Country = userData.Country.title ?? String.Empty
                    },
                Timezone = userData.timezone,
                Photo = userData.photo_max_orig
            };
        }

        public static explicit operator User(TwitterUser userData)
        {
            return new User
            {
                Name = new Name {FirstName = userData.Name ?? String.Empty, LastName = String.Empty},
                Birthday = String.Empty,
                Email = String.Empty,
                Gender = String.Empty,
                Location = new Location {City = userData.Location ?? String.Empty, Country = String.Empty},
                Timezone = -100,
                ScreenName = userData.ScreenName ?? String.Empty,
                Photo = userData.ProfileImageUrl ?? String.Empty
            };
        }
    }
}
