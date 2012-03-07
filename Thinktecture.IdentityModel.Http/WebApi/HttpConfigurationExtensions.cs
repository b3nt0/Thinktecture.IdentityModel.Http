using System.Web.Http;

namespace Thinktecture.IdentityModel.Http
{
    public static class HttpConfigurationExtensions
    {
        public static void SetAuthorizationManager(this HttpConfiguration configuration, IAuthorizationManager manager)
        {
            configuration.Properties[ApiAuthorizeAttribute.PropertyName] = manager;
        }
    }
}
