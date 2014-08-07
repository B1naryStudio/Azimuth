using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Azimuth.Infrastructure
{
    public class AzimuthIdentity : ClaimsIdentity
    {

        private static string[] _requiredClaims =
        {
            "AccessToken",
            "AccessTokenExpiresIn",
            "AccessTokenSecret",
            "ConsumerKey",
            "ConsumerSecret"
        };

        public static string[] RequiredClaims
        {
            get { return _requiredClaims; }
        }

        public string AccessToken
        {
            get
            {
                return GetClaim("AccessToken");
            }
        }

        public string AccessTokenExpiresIn
        {
            get
            {
                return GetClaim("AccessTokenExpiresIn");
            }
        }

        public string Email
        {
            get
            {
                return GetClaim(ClaimTypes.Email);
            }
        }

        public string AccessTokenSecret
        {
            get
            {
                return GetClaim("AccessTokenSecret");
            }
        }

        public string ConsumerKey
        {
            get
            {
                return GetClaim("ConsumerKey");
            }
        }

        public string ConsumerSecret
        {
            get
            {
                return GetClaim("ConsumerSecret");
            }
        }

        public string SocialNetworkName
        {
            get
            {
                var claim = Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                return claim == null ? String.Empty : claim.Issuer;
            }
        }

        public string SocialNetworkID
        {
            get
            {
                return GetClaim(ClaimTypes.NameIdentifier);
            }
        }

        public AzimuthIdentity(IEnumerable<Claim> claims) : base(claims)
        {
        }

        private string GetClaim(string claimType)
        {
            var claim = Claims.FirstOrDefault(c => c.Type == claimType);
            var data = (claim != null) ? claim.Value : String.Empty;
            return data;
        }
    }
}