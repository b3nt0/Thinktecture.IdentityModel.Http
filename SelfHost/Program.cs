using System;
using System.IdentityModel.Selectors;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.SelfHost;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Web;
using Thinktecture.IdentityModel.Http;
using Thinktecture.Samples.Resources;

namespace SelfHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new HttpSelfHostConfiguration("http://roadie:9000/services");
            var a = Assembly.Load("resources");
            
            ConfigureApis(config);

            config.Routes.MapHttpRoute(
                "API Default", 
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional });

            using (var server = new HttpSelfHostServer(config))
            {
                server.OpenAsync().Wait();

                Console.WriteLine("Press Enter to quit.");
                Console.ReadLine();
            }
        }

        private static void ConfigureApis(HttpConfiguration configuration)
        {
            configuration.MessageHandlers.Add(new AuthenticationHandler(ConfigureAuthentication()));
            configuration.SetAuthorizationManager(new AuthorizationManager());
        }

        private static AuthenticationConfiguration ConfigureAuthentication()
        {
            var config = new AuthenticationConfiguration();
            config.ClaimsAuthenticationManager = new ConsultantsClaimsTransformer();

            #region Basic Authentication
            config.Handler.AddBasicAuthenticationHandler((username, password) => username == password);
            #endregion

            #region IdSrv Simple Web Tokens
            config.Handler.AddSimpleWebTokenHandler(
                "IdSrv",
                "http://identity.thinktecture.com/trust",
                Thinktecture.Samples.Constants.Realm,
                "Dc9Mpi3jbooUpBQpB/4R7XtUsa3D/ALSjTVvK8IUZbg=");
            #endregion

            #region ACS Simple Web Tokens
            config.Handler.AddSimpleWebTokenHandler(
                "ACS",
                "https://" + Thinktecture.Samples.Constants.ACS + "/",
                Thinktecture.Samples.Constants.Realm,
                "yFvxu8Xkmo/xBSSPrzqZLSAiB4lgjR4PIi0Bn1RsUDI=");
            #endregion

            #region ADFS SAML tokens
            // SAML via ADFS
            var registry = new ConfigurationBasedIssuerNameRegistry();
            registry.AddTrustedIssuer("d1 c5 b1 25 97 d0 36 94 65 1c e2 64 fe 48 06 01 35 f7 bd db", "ADFS");

            var adfsConfig = new SecurityTokenHandlerConfiguration();
            adfsConfig.AudienceRestriction.AllowedAudienceUris.Add(new Uri(Thinktecture.Samples.Constants.Realm));
            adfsConfig.IssuerNameRegistry = registry;
            adfsConfig.CertificateValidator = X509CertificateValidator.None;

            // token decryption (read from configuration section)
            adfsConfig.ServiceTokenResolver = FederatedAuthentication.ServiceConfiguration.CreateAggregateTokenResolver();

            config.Handler.AddSaml11SecurityTokenHandler("AdfsSaml", adfsConfig);
            //manager.AddSaml2SecurityTokenHandler("AdfsSaml", adfsConfig);

            #endregion

            #region IdSrv SAML tokens
            // SAML via IdSrv
            var idsrvRegistry = new ConfigurationBasedIssuerNameRegistry();
            registry.AddTrustedIssuer("CD638612A35CD2F9232ECE36226B731FC666EF07", "Thinktecture IdSrv");

            var idsrvConfig = new SecurityTokenHandlerConfiguration();
            adfsConfig.AudienceRestriction.AllowedAudienceUris.Add(new Uri(Thinktecture.Samples.Constants.Realm));
            adfsConfig.IssuerNameRegistry = registry;
            adfsConfig.CertificateValidator = X509CertificateValidator.None;

            // token decryption (read from configuration section)
            adfsConfig.ServiceTokenResolver = FederatedAuthentication.ServiceConfiguration.CreateAggregateTokenResolver();

            config.Handler.AddSaml2SecurityTokenHandler("IdSrvSaml", adfsConfig);

            #endregion

            return config;
        }
    }
}
