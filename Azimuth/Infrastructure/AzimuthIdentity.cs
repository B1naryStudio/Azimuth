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

        public UserCredential UserCredential
        {
            get
            {
                return new UserCredential
                {
                    AccessToken = GetClaim("AccessToken"),
                    AccessTokenExpiresIn = GetClaim("AccessTokenExpiresIn"),
                    AccessTokenSecret = GetClaim("AccessTokenSecret"),
                    ConsumerKey = GetClaim("ConsumerKey"),
                    ConsumerSecret = GetClaim("ConsumerSecret"),
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

        public AzimuthIdentity(IEnumerable<Claim> claims) : base(claims)
        {
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
}