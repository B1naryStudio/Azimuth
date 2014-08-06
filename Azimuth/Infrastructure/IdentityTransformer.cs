using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;

namespace Azimuth.Infrastructure
{
    public class IdentityTransformer : ClaimsAuthenticationManager
    {

        public override ClaimsPrincipal Authenticate(string resourceName, ClaimsPrincipal incomingPrincipal)
        {
            var identity = incomingPrincipal.Identity as ClaimsIdentity;
            if (identity == null)
                throw new AuthenticationException();

            return CreatePrincipal(identity);
        }

        private ClaimsPrincipal CreatePrincipal(ClaimsIdentity identity)
        {
            var snClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            if (snClaim == null)
                throw new AuthenticationException();

            var newClaims = AzimuthIdentity.RequiredClaims.Select(identity.FindFirst).Where(c => c != null).ToList();
            newClaims.Add(snClaim);

            AzimuthIdentity azIdentity = new AzimuthIdentity(newClaims);

            return new ClaimsPrincipal(azIdentity);
        }
    }
}