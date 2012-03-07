using System;
using System.Linq;
using System.Security.Principal;
using Microsoft.IdentityModel.Claims;

namespace Thinktecture.Samples.Resources.Data
{
    public class Identity
    {
        public string Name { get; set; }
        public string AuthenticationType { get; set; }
        public bool IsAuthenticated { get; set; }
        public string ClrType { get; set; }

        public Claims Claims { get; set; }

        public Identity()
        { }

        public Identity(IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }

            ClrType = identity.GetType().FullName;

            if (!identity.IsAuthenticated)
            {
                IsAuthenticated = false;
                return;
            }

            Name = identity.Name;
            AuthenticationType = identity.AuthenticationType;
            IsAuthenticated = true;

            var claimsIdentity = identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                Claims = new Claims();
                claimsIdentity.Claims.ToList().ForEach(c => Claims.Add(new Claim
                            {
                                ClaimType = c.ClaimType,
                                Value = c.Value,
                                Issuer = c.Issuer,
                                OriginalIssuer = c.OriginalIssuer
                            }));
            }
        }
    }
}