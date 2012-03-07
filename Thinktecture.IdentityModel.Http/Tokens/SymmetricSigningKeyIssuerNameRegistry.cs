using System.Collections.Generic;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens;

namespace Thinktecture.IdentityModel.Http
{
    public class SymmetricSigningKeyIssuerNameRegistry : IssuerNameRegistry
    {
        Dictionary<string, string> _allowedIssuers = new Dictionary<string, string>();

        public void AddTrustedIssuer(string issuerUri, string issuerName)
        {
            _allowedIssuers.Add(issuerUri.ToLowerInvariant(), issuerName.ToLowerInvariant());
        }

        public override string GetIssuerName(SecurityToken securityToken)
        {
            var swt = securityToken as SimpleWebToken;
            if (swt == null)
            {
                return null;
            }

            string name;
            if (_allowedIssuers.TryGetValue(swt.Issuer.ToLowerInvariant(), out name))
            {
                return name;
            }

            return null;
        }
    }
}