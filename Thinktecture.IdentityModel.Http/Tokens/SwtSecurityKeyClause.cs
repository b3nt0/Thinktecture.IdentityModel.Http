using System.IdentityModel.Tokens;

namespace Thinktecture.IdentityModel.Http
{
    /// <summary>
    /// The <see cref="SwtSecurityTokenHandler"/> passes an instance of this clause to 
    /// the <see cref="SwtIssuerTokenResolver"/> so that it knows it's an SWT that has 
    /// already been verified against the <see cref="SwtIssuerNameRegistry"/> trusted 
    /// issuers list. 
    /// </summary>
    /// <remarks>
    /// Because we only support one symmetric <see cref="SecurityKey"/> 
    /// for SWT, we don't need to differentiate between issuers.
    /// </remarks>
    internal class SwtSecurityKeyClause : SecurityKeyIdentifierClause
    {
        public string Issuer { get; set; }

        public SwtSecurityKeyClause(string issuer)
            : base("SWT")
        {
            Issuer = issuer;
        }
    }
}
