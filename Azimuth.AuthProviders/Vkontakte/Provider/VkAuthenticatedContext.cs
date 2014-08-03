using System;
using System.Globalization;
using System.Security.Claims;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;
using Newtonsoft.Json.Linq;

namespace Azimuth.AuthProviders.Vkontakte.Provider
{
    /// <summary>
    /// Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/>.
    /// </summary>
    public class VkAuthenticatedContext : BaseContext
    {
        /// <summary>
        /// Initializes a <see cref="VkAuthenticatedContext"/>
        /// </summary>
        /// <param name="context">The OWIN environment</param>
        /// <param name="user">The JSON-serialized user info</param>
        /// <param name="accessToken">Access token</param>
        /// <param name="expires">Seconds until expiration</param>
        public VkAuthenticatedContext(IOwinContext context, JObject user, string accessToken, string expires)
            : base(context)
        {
            //User = user;
            AccessToken = accessToken;

            //get user data from --> {"response" : [{...<user data>...}] }
            var temp = JArray.Parse(user["response"].ToString()).ToString();
            temp = temp.Substring(3, temp.Length - 6);
            User = JObject.Parse(temp);

            int expiresValue;
            if (Int32.TryParse(expires, NumberStyles.Integer, CultureInfo.InvariantCulture, out expiresValue))
            {
                ExpiresIn = TimeSpan.FromSeconds(expiresValue);
            }

            Id = TryGetValue(User, "uid");
            Name = TryGetValue(User, "first_name");
            LastName = TryGetValue(User, "last_name");
            UserName = TryGetValue(User, "screen_name");
            Nickname = TryGetValue(User, "nickname");
            Email = TryGetValue(User, "email");
            Link = TryGetValue(User, "photo_200");
            Sex = TryGetValue(User, "sex");
            Bdate = TryGetValue(User, "bdate");
            City = TryGetValue(User, "city");
            Country = TryGetValue(User, "country");
            Timezone = TryGetValue(User, "timezone");
        }

        /// <summary>
        /// Gets the document with user info
        /// </summary>
        public JObject User { get; private set; }

        /// <summary>
        /// Gets the access token
        /// </summary>
        public string AccessToken { get; private set; }

        /// <summary>
        /// Gets the access token expiration time
        /// </summary>
        public TimeSpan? ExpiresIn { get; set; }

        /// <summary>
        /// Gets the user ID
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the user's name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the user's last name
        /// </summary>
        public string LastName { get; private set; }

        /// <summary>
        /// Gets the user's full name
        /// </summary>
        public string FullName
        {
            get
            {
                return Name + " " + LastName;
            }
        }


        /// <summary>
        /// Gets the user's DefaultName
        /// </summary>
        public string DefaultName
        {
            get
            {
                if (!String.IsNullOrEmpty(UserName))
                    return UserName;

                if (!String.IsNullOrEmpty(Nickname))
                    return Nickname;

                return FullName;
            }
        }

        /// <summary>
        /// Get's the user's Email
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// Gets the user's picture link
        /// </summary>
        public string Link { get; private set; }

        /// <summary>
        /// Gets the username
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Gets the Nickname
        /// </summary>
        public string Nickname { get; private set; }

        public string Sex { get; private set; }
        public string Bdate { get; private set; }
        public string City { get; private set; }
        public string Country { get; private set; }
        public string Timezone { get; private set; }

        /// <summary>
        /// Gets the <see cref="ClaimsIdentity"/> representing the user
        /// </summary>
        public ClaimsIdentity Identity { get; set; }

        /// <summary>
        /// Gets or sets a property bag for common authentication properties
        /// </summary>
        public AuthenticationProperties Properties { get; set; }

        private string TryGetValue(JObject user, string propertyName)
        {
            JToken value;
            return user.TryGetValue(propertyName, out value) ? value.ToString() : null;
        }
    }
}
