using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.SecurityTokenService;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml11;
using Microsoft.IdentityModel.Web;
using Thinktecture.IdentityModel.Extensions;
using Thinktecture.IdentityModel.Http;

namespace Tests
{
    internal static class Factory
    {
        public static HttpRequestMessage GetDefaultRequest()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://test/");
            return request;
        }

        public static HttpConfiguration GetDefaultConfiguration()
        {
            var authHandler = GetDefaultAuthenticationHandler();

            var httpConfig = new HttpConfiguration();
            httpConfig.MessageHandlers.Add(authHandler);

            return httpConfig;
        }

        private static AuthenticationHandler GetDefaultAuthenticationHandler()
        {
            var authConfig = new AuthenticationConfiguration
            {
                InheritHostClientIdentity = false
            };

            #region Basic Authentication
            authConfig.Handler.AddBasicAuthenticationHandler((userName, password) => { return userName == password; });
            #endregion

            #region SWT
            authConfig.Handler.AddSimpleWebTokenHandler(
                "SWT", 
                Constants.Issuer,
                Constants.Realm,
                "Dc9Mpi3jbooUpBQpB/4R7XtUsa3D/ALSjTVvK8IUZbg=");
            #endregion

            #region SAML tokens
            var registry = new ConfigurationBasedIssuerNameRegistry();
            registry.AddTrustedIssuer("D263DDCF598E716F0037380796A4A62DF017ADB8", "TEST");

            var adfsConfig = new SecurityTokenHandlerConfiguration();
            adfsConfig.AudienceRestriction.AllowedAudienceUris.Add(new Uri(Constants.Realm));
            adfsConfig.IssuerNameRegistry = registry;
            adfsConfig.CertificateValidator = X509CertificateValidator.None;

            // token decryption (read from configuration section)
            adfsConfig.ServiceTokenResolver = FederatedAuthentication.ServiceConfiguration.CreateAggregateTokenResolver();

            authConfig.Handler.AddSaml11SecurityTokenHandler("SAML", adfsConfig);
            //manager.AddSaml2SecurityTokenHandler("AdfsSaml", adfsConfig);

            #endregion

            var authHandler = new AuthenticationHandler(authConfig);
            return authHandler;
        }

        public static HttpServer GetDefaultServer()
        {
            return new HttpServer(GetDefaultConfiguration(), new LoopBackHandler(req => new HttpResponseMessage()));
        }

        public static string CreateSaml11Token(string name)
        {
            var id = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, name) });

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = id,
                AppliesToAddress = Constants.Realm,
                TokenIssuerName = Constants.Issuer,
                SigningCredentials = GetSamlSigningCredential()
            };

            var handler = new Saml11SecurityTokenHandler();
            handler.Configuration = new SecurityTokenHandlerConfiguration();

            var token = handler.CreateToken(descriptor);
            return token.ToTokenXmlString();
        }

        private static SigningCredentials GetSamlSigningCredential()
        {
            var cert = new X509Certificate2("test.pfx", "abc!123");
            return new X509SigningCredentials(cert);
        }
    }
}
