﻿using System;
using System.IdentityModel.Tokens;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.ServiceModel.Security;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel.Protocols.WSTrust.Bindings;
using Microsoft.IdentityModel.SecurityTokenService;
using Thinktecture.IdentityModel.Utility;
using Thinktecture.Samples;
using Thinktecture.Samples.Resources.Data;

namespace AdfsSamlClient
{
    class Program
    {
        static Uri _baseAddress = new Uri(Constants.ServiceBaseAddressWebHost);
        
        static EndpointAddress _idpEndpoint =
            new EndpointAddress("https://adfs.leastprivilege.vm/adfs/services/trust/13/windowstransport");

        static void Main(string[] args)
        {
            while (true)
            {
                Identity id = null;
                Console.Clear();

                Helper.Timer(() =>
                {
                    var token = GetIdentityToken();
                    id = CallService(token);
                });

                id.ShowConsole();

                Console.ReadLine();
            }
        }

        private static string GetIdentityToken()
        {
            "Requesting identity token".ConsoleYellow();

            var factory = new WSTrustChannelFactory(
                new WindowsWSTrustBinding(SecurityMode.Transport),
                _idpEndpoint);
            factory.TrustVersion = TrustVersion.WSTrust13;

            var rst = new RequestSecurityToken
            {
                RequestType = RequestTypes.Issue,
                KeyType = KeyTypes.Bearer,
                /* TokenType = Microsoft.IdentityModel.Tokens.SecurityTokenTypes.Saml2TokenProfile11, */
                AppliesTo = new EndpointAddress(Constants.Realm)
            };

            var token = factory.CreateChannel().Issue(rst) as GenericXmlSecurityToken;
            return token.TokenXml.OuterXml;
        }

        private static Identity CallService(string saml)
        {
            "Calling service".ConsoleYellow();

            var client = new HttpClient { BaseAddress = _baseAddress };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("AdfsSaml", saml);

            var response = client.GetAsync("identity").Result;
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsAsync<Identity>().Result;
        }
    }
}
