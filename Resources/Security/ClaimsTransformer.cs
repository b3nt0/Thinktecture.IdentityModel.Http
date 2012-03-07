using System.Collections.Generic;
using Microsoft.IdentityModel.Claims;

namespace Thinktecture.Samples.Resources
{
    public class ClaimsTransformer : ClaimsAuthenticationManager
    {
        public override IClaimsPrincipal Authenticate(string resourceName, IClaimsPrincipal incomingPrincipal)
        {
            if (!incomingPrincipal.Identity.IsAuthenticated)
            {
                return base.Authenticate(resourceName, incomingPrincipal);
            }

            return CreateClientIdentity(incomingPrincipal.Identity as IClaimsIdentity);
        }

        private IClaimsPrincipal CreateClientIdentity(IClaimsIdentity id)
        {
            // hard coded for demo purposes
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, id.Name),
                new Claim(ClaimTypes.Role, "Users"),
                new Claim(ClaimTypes.Role, "Geek"),
                new Claim(ClaimTypes.Email, id.Name + "@thinktecture.com")
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Federation");
            return ClaimsPrincipal.CreateFromIdentity(claimsIdentity);
        }
    }
}
