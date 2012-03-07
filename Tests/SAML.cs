using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Microsoft.IdentityModel.Claims;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class SAML
    {
        [TestMethod]
        public void ValidSaml11Token()
        {
            var token = Factory.CreateSaml11Token("test");
            
            var client = new HttpClient(Factory.GetDefaultServer());
            var request = Factory.GetDefaultRequest();
            request.Headers.Authorization = new AuthenticationHeaderValue("SAML", token);

            var response = client.SendAsync(request).Result;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);  
        }

        [TestMethod]
        public void ValidSaml11TokenCheckIdentity()
        {
            var token = Factory.CreateSaml11Token("test");

            var client = new HttpClient(Factory.GetDefaultServer());
            var request = Factory.GetDefaultRequest();
            request.Headers.Authorization = new AuthenticationHeaderValue("SAML", token);

            var response = client.SendAsync(request).Result;

            var id = request.GetUserPrincipal().Identity as IClaimsIdentity;
            Assert.IsNotNull(id, "Identity is null");

            Assert.IsTrue(id.IsAuthenticated, "Identity is anonymous");
            Assert.AreEqual("test", id.Name);
        }

        [TestMethod]
        public void EmptyCredentials()
        {
            var client = new HttpClient(Factory.GetDefaultServer());
            var request = Factory.GetDefaultRequest();
            request.Headers.Authorization = new AuthenticationHeaderValue("SAML");

            var response = client.SendAsync(request).Result;
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public void MalformedCredentials()
        {
            var client = new HttpClient(Factory.GetDefaultServer());
            var request = Factory.GetDefaultRequest();
            request.Headers.Authorization = new AuthenticationHeaderValue("SAML", "invalid");

            var response = client.SendAsync(request).Result;
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
