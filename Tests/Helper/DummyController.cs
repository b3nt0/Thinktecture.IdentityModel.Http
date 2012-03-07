using System.Web.Http;

namespace Tests
{
    public class DummyController : ApiController
    {
        public string Get()
        {
            return "OK";
        }
    }
}
