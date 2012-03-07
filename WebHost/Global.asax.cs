using System;
using System.IdentityModel.Selectors;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Web;
using Thinktecture.IdentityModel.Http;
using Thinktecture.Samples.Resources;

namespace Thinktecture.Samples
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            ConfigureApis(GlobalConfiguration.Configuration);
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            BundleTable.Bundles.RegisterTemplateBundles();
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        private void ConfigureApis(HttpConfiguration configuration)
        {
            configuration.MessageHandlers.Add(new AuthenticationHandler(ConfigureAuthentication()));
            configuration.SetAuthorizationManager(new AuthorizationManager());
        }

        private AuthenticationConfiguration ConfigureAuthentication()
        {
            var config = new AuthenticationConfiguration
            {
                // sample claims transformation for consultants sample, comment out to see raw claims
                ClaimsAuthenticationManager = new ConsultantsClaimsTransformer(),

                // value of the www-authenticate header, if not set, the first scheme added to the handler collection is used
                DefaultAuthenticationScheme = "Basic"
            };

            #region Basic Authentication
            config.Handler.AddBasicAuthenticationHandler((username, password) => username == password);
            #endregion

            #region IdSrv Simple Web Tokens
            config.Handler.AddSimpleWebTokenHandler(
                "IdSrv",
                "http://identity.thinktecture.com/trust",
                Constants.Realm,
                "Dc9Mpi3jbooUpBQpB/4R7XtUsa3D/ALSjTVvK8IUZbg=");
            #endregion

            #region ACS Simple Web Tokens
            config.Handler.AddSimpleWebTokenHandler(
                "ACS",
                "https://" + Constants.ACS + "/",
                Constants.Realm,
                "yFvxu8Xkmo/xBSSPrzqZLSAiB4lgjR4PIi0Bn1RsUDI=");
            #endregion

            #region ADFS SAML tokens
            // SAML via ADFS
            var registry = new ConfigurationBasedIssuerNameRegistry();
            registry.AddTrustedIssuer("d1 c5 b1 25 97 d0 36 94 65 1c e2 64 fe 48 06 01 35 f7 bd db", "ADFS");

            var adfsConfig = new SecurityTokenHandlerConfiguration();
            adfsConfig.AudienceRestriction.AllowedAudienceUris.Add(new Uri(Constants.Realm));
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
            adfsConfig.AudienceRestriction.AllowedAudienceUris.Add(new Uri(Constants.Realm));
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