using System.Web.Http;
using System.Web.Http.Controllers;
using Microsoft.IdentityModel.Claims;
using Thinktecture.IdentityModel.Extensions;
using Thinktecture.IdentityModel.Http;
using Thinktecture.Samples.Resources.Data;
using System.Linq;

namespace Thinktecture.Samples.Resources
{
    public class AuthorizationManager : MethodResourceAuthorizationManager
    {
        IConsultantsRepository _repository;

        public AuthorizationManager()
        {
            _repository = new InMemoryConsultantsRepository();
        }

        public AuthorizationManager(IConsultantsRepository repository)
        {
            _repository = repository; ;
        }

        //
        // POST /consultants
        //
        public bool PostConsultants(HttpActionContext context)
        {
            var principal = context.Request.GetUserClaims();

            // authorize based on reportsTo claim
            return principal.ClaimExists(Constants.ClaimTypes.ReportsTo, "christian");
        }

        //
        // PUT /consultants
        //
        public bool PutConsultants(HttpActionContext context)
        {
            var principal = context.Request.GetUserClaims();

            // authorize based on reportsTo claim
            if (!principal.ClaimExists(Constants.ClaimTypes.ReportsTo, "christian"))
            {
                return false;
            }

            // if no id is specified, nothing to do here
            if (context.ControllerContext.RouteData.Values.ContainsKey("id"))
            {
                return CheckOwnership(int.Parse(context.ControllerContext.RouteData.Values["id"].ToString()), principal);
            }

            return true;
        }

        //
        // DELETE /consultants
        //
        public bool DeleteConsultants(HttpActionContext context)
        {
            var principal = context.Request.GetUserClaims();

            // authorize based on authentication method
            if (!principal.ClaimExists(ClaimTypes.AuthenticationMethod, AuthenticationMethods.X509))
            {
                return false;
            }

            // authorize based on role membership
            //return principal.IsInRole("Owner");

            // if no id is specified, nothing to do here
            if (context.ControllerContext.RouteData.Values.ContainsKey("id"))
            {
                return CheckOwnership(int.Parse(context.ControllerContext.RouteData.Values["id"].ToString()), principal);
            }

            return true;
        }

        private bool CheckOwnership(int id, IClaimsPrincipal principal)
        {
            var oldConsultant = _repository.GetAll().FirstOrDefault(c => c.ID == id);

            if (oldConsultant == null)
            {
                return true;
            }

            // check if client is allowed to update consultant
            // only the record creator can update
            return oldConsultant.Owner.Equals(principal.Identity.Name);
        }
    }
}
