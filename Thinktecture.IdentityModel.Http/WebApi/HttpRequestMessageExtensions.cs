using System.Net.Http;
using Microsoft.IdentityModel.Claims;

namespace System.Web.Http
{
    public static class ThinktectureHttpRequestMessageExtensions
    {
        public static IClaimsPrincipal GetUserClaims(this HttpRequestMessage request)
        {
            var id = request.GetUserPrincipal() as IClaimsPrincipal;

            if (id == null)
            {
                throw new InvalidOperationException("Principal is not a ClaimsPrincipal");
            }

            return id;
        }
    }
}
