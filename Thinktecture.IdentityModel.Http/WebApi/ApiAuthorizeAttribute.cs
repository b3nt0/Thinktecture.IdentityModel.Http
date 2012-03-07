using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Configuration;

namespace Thinktecture.IdentityModel.Http
{
    public class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        public const string PropertyName = "Thinktecture.IdentityModel.Http.IAuthorizationManager";

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }

            if (!SkipAuthorization(actionContext) && !AuthorizeCore(actionContext))
            {
                this.HandleUnauthorizedRequest(actionContext);
            }
        }

        protected virtual bool AuthorizeCore(HttpActionContext actionContext)
        {
            if (actionContext.ControllerContext.Configuration.Properties.ContainsKey(PropertyName))
            {
                var authZmanager = actionContext.ControllerContext.Configuration.Properties[PropertyName] as IAuthorizationManager;

                if (authZmanager != null)
                {
                    return authZmanager.CheckAccess(actionContext);
                }
            }

            throw new ConfigurationErrorsException("No authorization manager configured");
        }

        protected virtual bool SkipAuthorization(HttpActionContext actionContext)
        {
            if (!actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any<AllowAnonymousAttribute>())
            {
                return actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any<AllowAnonymousAttribute>();
            }

            return true;
        }
    }
}
