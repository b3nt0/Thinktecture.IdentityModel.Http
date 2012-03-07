using System.IO;
using System.Xml;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;

namespace Thinktecture.IdentityModel.Http
{
    public class WebSaml2SecurityTokenHandler : Saml2SecurityTokenHandler, IWebSecurityTokenHandler
    {
        public WebSaml2SecurityTokenHandler()
            : base()
        { }

        public WebSaml2SecurityTokenHandler(SecurityTokenHandlerConfiguration configuration)
            : base()
        {
            Configuration = configuration;
        }

        public IClaimsPrincipal ValidateWebToken(string token)
        {
            var securityToken = ReadToken(new XmlTextReader(new StringReader(token)));
            return ClaimsPrincipal.CreateFromIdentities(ValidateToken(securityToken));
        }
    }
}
