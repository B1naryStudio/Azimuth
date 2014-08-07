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
            var myPlace = ((userData.placesLived ?? new GoogleUserData.GoogleLocation[] { }).FirstOrDefault(p => p.primary) ?? new GoogleUserData.GoogleLocation { value = String.Empty }).value;
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

            if (userData.location != null)
            {
                if (userData.location.name != null)
                {
                    city = userData.location.name.Split(',')[0];
                    country = userData.location.name.Split(' ')[1];
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
                Photo = userData.picture.data.url
            };
        }

        public static explicit operator User(VKUserdata.Response userData)
        {
            return new User
            {
                Name =
                    new Name
                    {
                        FirstName = userData.response[0].first_name ?? String.Empty,
                        LastName = userData.response[0].last_name ?? String.Empty
                    },
                ScreenName = userData.response[0].screen_name ?? String.Empty,
                Gender = (userData.response[0].sex != 0) ? userData.response[0].sex.ToString() : String.Empty,
                Birthday = userData.response[0].bdate ?? String.Empty,
                Email = userData.response[0].email ?? String.Empty,
                Location =
                    new Location
                    {
                        City = userData.response[0].city.title ?? String.Empty,
                        Country = userData.response[0].country.title ?? String.Empty
                    },
                Timezone = userData.response[0].timezone,
                Photo = userData.response[0].photo_max_orig
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
