using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Azimuth.Infrastructure
{
    public class AzimuthIdentity : ClaimsIdentity
    {

        private static readonly string[] _requiredClaims =
        {
            AzimuthClaims.ACCESS_TOKEN,
            AzimuthClaims.ACCESS_TOKEN_EXPIRES_IN,
            AzimuthClaims.ACCESS_TOKEN_SECRET,
            AzimuthClaims.CONSUMER_KEY,
            AzimuthClaims.CONSUMER_SECRET
        };

        public UserCredential UserCredential
        {
            get
            {
                return new UserCredential
                {
                    AccessToken = GetClaim(AzimuthClaims.ACCESS_TOKEN),
                    AccessTokenExpiresIn = GetClaim(AzimuthClaims.ACCESS_TOKEN_EXPIRES_IN),
                    AccessTokenSecret = GetClaim(AzimuthClaims.ACCESS_TOKEN_SECRET),
                    ConsumerKey = GetClaim(AzimuthClaims.CONSUMER_KEY),
                    ConsumerSecret = GetClaim(AzimuthClaims.CONSUMER_SECRET),
                    SocialNetworkId = GetSocialNetworkId(),
                    SocialNetworkName = GetSocialNetworkName(),
                    Email = GetClaim(ClaimTypes.Email)
                };
            }
        }

        public static string[] RequiredClaims
        {
            get { return _requiredClaims; }
        }

        public AzimuthIdentity(IEnumerable<Claim> claims, string type) : base(claims, type)
        {
        }

        public static AzimuthIdentity Current
        {
            get
            {
                var authManager = HttpContext.Current.GetOwinContext().Authentication;
                var logged = authManager.GetExternalIdentityAsync(DefaultAuthenticationTypes.ApplicationCookie).Result;
                return logged != null ? new AzimuthIdentity(logged.Claims, DefaultAuthenticationTypes.ApplicationCookie) : null;
            }
        }

        private string GetClaim(string claimType)
        {
            var claim = Claims.FirstOrDefault(c => c.Type == claimType);
            var data = (claim != null) ? claim.Value : String.Empty;
            return data;
        }


        private string GetSocialNetworkName()
        {
            var claim = Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return claim == null ? String.Empty : claim.Issuer;
        }

        private string GetSocialNetworkId()
        {
            return GetClaim(ClaimTypes.NameIdentifier);
        }
    }

    public static class AzimuthClaims
    {
        public const string ACCESS_TOKEN = "AccessToken";

        public const string ACCESS_TOKEN_EXPIRES_IN = "AccessTokenExpiresIn";
        
        public const string ACCESS_TOKEN_SECRET = "AccessTokenSecret";
        
        public const string CONSUMER_KEY = "ConsumerKey";

        public const string CONSUMER_SECRET = "ConsumerSecret";
    }
}