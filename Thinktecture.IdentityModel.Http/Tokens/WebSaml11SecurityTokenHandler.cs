using System.IO;
using System.Xml;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml11;

namespace Thinktecture.IdentityModel.Http
{
    public class WebSaml11SecurityTokenHandler : Saml11SecurityTokenHandler, IWebSecurityTokenHandler
    {
        public WebSaml11SecurityTokenHandler() 
            : base()
        { }

        public WebSaml11SecurityTokenHandler(SecurityTokenHandlerConfiguration configuration) 
            : base()
        {
            Configuration = configuration;
        }

        public IClaimsPrincipal ValidateWebToken(string token)
        {
            var securityToken = ContainingCollection.ReadToken(new XmlTextReader(new StringReader(token)));
            return ClaimsPrincipal.CreateFromIdentities(ValidateToken(securityToken));
        }
    }
}
