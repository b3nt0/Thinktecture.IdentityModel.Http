using System.Web.Http.Controllers;

namespace Thinktecture.IdentityModel.Http
{
    public abstract class MethodResourceAuthorizationManager : ReflectionBasedAuthorizationManager
    {
        // builds the method name based on HTTP method and resource, e.g. GetCustomer
        protected override string BuildMethodName(HttpActionContext context)
        {
            var method = Format(context.Request.Method.ToString());
            var resource = Format(context.ControllerContext.ControllerDescriptor.ControllerName);

            return method + resource;
        }

        protected virtual string Format(string input)
        {
            // get the first letter an make it uppercase
            var f = input.Substring(0, 1).ToUpperInvariant();

            // get the rest lowercase
            var r = input.Substring(1).ToLowerInvariant();

            return f + r;
        }
    }
}
