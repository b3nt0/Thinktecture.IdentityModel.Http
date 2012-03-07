using Microsoft.IdentityModel.Claims;

namespace Thinktecture.IdentityModel.Http
{
    interface IWebSecurityTokenHandler
    {
        IClaimsPrincipal ValidateWebToken(string token);
    }
}
