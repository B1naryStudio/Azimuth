using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.AspNet.Identity;

namespace Azimuth.Infrastructure.Concrete
{
    public class IdentityTransformer : ClaimsAuthenticationManager
    {
        public override ClaimsPrincipal Authenticate(string resourceName, ClaimsPrincipal incomingPrincipal)
        {
            var identity = incomingPrincipal.Identity as ClaimsIdentity;
            if (identity == null)
            {
                throw new AuthenticationException();
            }

            return CreatePrincipal(identity);
        }

        private ClaimsPrincipal CreatePrincipal(ClaimsIdentity identity)
        {
            var snClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            var userEmail = identity.FindFirst(ClaimTypes.Email);
            if (snClaim == null)
            {
                throw new AuthenticationException();
            }

            var newClaims = AzimuthIdentity.RequiredClaims.Select(identity.FindFirst).Where(c => c != null).ToList();
            newClaims.Add(snClaim);
            if (userEmail != null)
            {
                newClaims.Add(userEmail);
            }

            var azIdentity = new AzimuthIdentity(newClaims,
                                        DefaultAuthenticationTypes.ApplicationCookie);

            return new ClaimsPrincipal(azIdentity);
        }
    }
}