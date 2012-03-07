using System;
using System.IdentityModel.Tokens;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel.Protocols.WSTrust.Bindings;
using Microsoft.IdentityModel.SecurityTokenService;
using Thinktecture.IdentityModel.Http;
using Thinktecture.IdentityModel.Http.OAuth2;
using Thinktecture.IdentityModel.Utility;
using Thinktecture.Samples;
using Thinktecture.Samples.Resources.Data;

namespace AcsSamlToSwtClient
{
    class Program
    {
        static EndpointAddress _idpEndpoint =
            new EndpointAddress("https://" + Constants.IdSrv + "/idsrv/issue/wstrust/mixed/certificate");

        static EndpointAddress _acsBaseAddress = new EndpointAddress("https://" + Constants.ACS + "/");

        static Uri _acsWrapEndpoint = new Uri("https://" + Constants.ACS + "/WRAPv0.9");
        static Uri _acsOAuth2Endpoint = new Uri("https://" + Constants.ACS + "/v2/OAuth2-13");

        static Uri _baseAddress = new Uri(Constants.ServiceBaseAddressWebHost);

        static void Main(string[] args)
        {
            while (true)
            {
                Identity id = null;
                Console.Clear();

                Helper.Timer(() =>
                {
                    var identityToken = GetIdentityToken();
                    var serviceToken = GetServiceTokenOAuth2(identityToken);

                    id = CallService(serviceToken);
                });

                id.ShowConsole();

                Console.ReadLine();
            }
        }

        private static string GetIdentityToken()
        {
            "Requesting identity token".ConsoleYellow();

            var factory = new WSTrustChannelFactory(
                new CertificateWSTrustBinding(SecurityMode.TransportWithMessageCredential),
                _idpEndpoint);
            factory.TrustVersion = TrustVersion.WSTrust13;

            factory.Credentials.ClientCertificate.SetCertificate(
                StoreLocation.CurrentUser,
                StoreName.My,
                X509FindType.FindBySubjectDistinguishedName,
                "CN=Client");

            var rst = new RequestSecurityToken
            {
                RequestType = RequestTypes.Issue,
                KeyType = KeyTypes.Bearer,
                AppliesTo = _acsBaseAddress
            };

            var token = factory.CreateChannel().Issue(rst) as GenericXmlSecurityToken;

            return token.TokenXml.OuterXml;
        }

        private static string GetServiceTokenWrap(string samlToken)
        {
            "Requesting service token".ConsoleYellow();

            var client = new WrapClient(_acsWrapEndpoint);
            return client.IssueAssertion(samlToken, "SAML", new Uri(Constants.Realm)).RawToken;
        }

        private static string GetServiceTokenOAuth2(string samlToken)
        {
            "Requesting service token".ConsoleYellow();

            var client = new OAuth2Client(_acsOAuth2Endpoint);
            return client.RequestAccessTokenAssertion(
                samlToken, 
                Microsoft.IdentityModel.Tokens.SecurityTokenTypes.Saml2TokenProfile11, 
                Constants.Realm).AccessToken;
        }

        private static Identity CallService(string swt)
        {
            "Calling service".ConsoleYellow();

            var client = new HttpClient { BaseAddress = _baseAddress };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("ACS", swt);

            var response = client.GetAsync("identity").Result;
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsAsync<Identity>().Result;
        }
    }
}
