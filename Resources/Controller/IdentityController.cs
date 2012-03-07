using System.Threading;
using System.Web.Http;
using Thinktecture.Samples.Resources.Data;

namespace Thinktecture.Samples.Resources
{
    public class IdentityController : ApiController
    {
        public Identity Get()
        {
            return new Identity(Thread.CurrentPrincipal.Identity);
        }
    }
}