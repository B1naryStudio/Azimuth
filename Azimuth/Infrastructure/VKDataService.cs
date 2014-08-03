using System;
using System.IO;
using System.Net;
using System.Text;
using Azimuth.DataAccess.Entities;
using Newtonsoft.Json.Linq;
using NHibernate.Cache.Entry;

namespace Azimuth.Infrastructure
{
    public class VkDataService
    {
        private readonly string _userId;
        private readonly string _accessToken;

        public VkDataService(string userId, string accessToken = "")
        {
            _userId = userId;
            _accessToken = accessToken;
        }

        public User GetUserInfo()
        {
            var userInfoUrl = GetUserInfoUrl(_userId, _accessToken);
            var response = GetRequest(userInfoUrl);

            var sJObject = JObject.Parse(response);
            var userInfo = sJObject["response"][0];

            var user = new User
            {
                Name = new Name()
                {
                    FirstName = userInfo["first_name"].ToString(),
                    LastName = userInfo["last_name"].ToString()
                },
                ScreenName = userInfo["screen_name"].ToString(),
                Location = new Location()
                {
                    City = (userInfo["city"] != null) ? userInfo["city"]["title"].ToString() : String.Empty,
                    Country = (userInfo["country"] != null) ? userInfo["country"]["title"].ToString() : String.Empty
                },
                Photo = (userInfo["photo_400_orig"] != null) ? userInfo["photo_400_orig"].ToString() : String.Empty,
                Birthday = (userInfo["bdate"] != null) ? userInfo["bdate"].ToString() : String.Empty
            };

            var gender = userInfo["sex"];

            if (gender != null)
            {
                switch (userInfo["sex"].ToString())
                {
                    case "0":
                        user.Gender = "None";
                        break;
                    case "1":
                        user.Gender = "Female";
                        break;
                    case "2":
                        user.Gender = "Male";
                        break;
                }    
            }

            // TODO: Get access token
            // Need access token for timezone

            //int timezone;
            //var timeS = userObj["timezone"].ToString();
            //int.TryParse(timeS, out timezone);
            //user.Timezone = timezone;
            
            return user;
        }

        private static string GetUserInfoUrl(string userId, string accessToken)
        {
            return String.Format(
                @"https://api.vk.com/method/users.get?user_id={0}&fields=screen_name,bdate,sex,city,country,photo_400_orig,timezone&v=5.23&access_token={1}",
                userId,
                accessToken);
        }

        private static string GetRequest(string url)
        {
            try
            {
                var wr = WebRequest.Create(url);

                var objStream = wr.GetResponse().GetResponseStream();

                if (objStream == null)
                    return "";

                var objReader = new StreamReader(objStream);

                var sb = new StringBuilder();
                while (true)
                {
                    string line = objReader.ReadLine();
                    if (line != null) sb.Append(line);

                    else
                    {
                        return sb.ToString();
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}