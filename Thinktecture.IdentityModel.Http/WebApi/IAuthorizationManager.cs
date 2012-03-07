using System.Web.Http.Controllers;

namespace Thinktecture.IdentityModel.Http
{
    public interface IAuthorizationManager
    {
        bool CheckAccess(HttpActionContext context);
    }
}
