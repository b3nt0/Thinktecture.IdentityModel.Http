using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Microsoft.IdentityModel.Claims;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class General
    {
        [TestMethod]
        public void NoCredential()
        {
            var client = new HttpClient(Factory.GetDefaultServer());
            var request = Factory.GetDefaultRequest();

            var response = client.SendAsync(request).Result;
            
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var id = request.GetUserPrincipal().Identity as IClaimsIdentity;
            Assert.IsNotNull(id, "Identity is null");

            Assert.IsFalse(id.IsAuthenticated, "Identity is not anonymous");
        }

        [TestMethod]
        public void UnknownScheme()
        {
            var client = new HttpClient(Factory.GetDefaultServer());
            var request = Factory.GetDefaultRequest();
            request.Headers.Authorization = new AuthenticationHeaderValue("unknown", "foo");

            var response = client.SendAsync(request).Result;
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
