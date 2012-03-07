using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Thinktecture.IdentityModel.Http;
using Thinktecture.Samples.Resources.Data;
using System.Threading;

namespace Thinktecture.Samples.Resources
{
    [ApiAuthorize]
    public class ConsultantsController : ApiController
    {
        IConsultantsRepository _repository;

        public ConsultantsController()
        {
            _repository = new InMemoryConsultantsRepository();
        }

        [AllowAnonymous]
        public IQueryable<Consultant> Get()
        {
            return _repository.GetAll().AsQueryable();
        }

        [AllowAnonymous]
        public Consultant Get(int id)
        {
            var consultant = _repository.GetAll().FirstOrDefault(c => c.ID == id);

            if (consultant == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return consultant;
        }

        public HttpResponseMessage Post(Consultant consultant)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            consultant.Owner = Thread.CurrentPrincipal.Identity.Name;
            var id = _repository.Add(consultant);

            var response = new HttpResponseMessage(HttpStatusCode.Created);
            response.Headers.Location = new Uri(Request.RequestUri.AbsoluteUri + "/" + id.ToString());
            
            return response;
        }

        public void Put([FromUri] int id, Consultant consultant)
        {
            // check if consultant exists
            var oldConsultant = _repository.GetAll().FirstOrDefault(c => c.ID == id);

            if (oldConsultant == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            consultant.ID = id;
            consultant.Owner = Thread.CurrentPrincipal.Identity.Name;
            _repository.Update(consultant);
        }
    }
}
