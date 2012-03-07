using System.Linq;
using Microsoft.IdentityModel.Claims;

namespace Thinktecture.IdentityModel.Http
{
    public class AuthenticationConfiguration
    {
        WebSecurityTokenHandlerCollectionManager _manager;
        string _defaultAuthenticationScheme;

        public ClaimsAuthenticationManager ClaimsAuthenticationManager { get; set; }
        public bool UseDefaultClaimsAuthenticationManager { get; set; }
        public bool InheritHostClientIdentity { get; set; }
        
        public string DefaultAuthenticationScheme
        {
            set { _defaultAuthenticationScheme = value;  }
            get
            {
                if (!string.IsNullOrEmpty(_defaultAuthenticationScheme))
                {
                    return _defaultAuthenticationScheme;
                }

                if (_manager.Count > 0)
                {
                    return _manager.RegisteredSchemes.First();
                }

                return "unknown";
            }
        }

        public WebSecurityTokenHandlerCollectionManager Handler
        {
            get { return _manager; }
        }

        public AuthenticationConfiguration()
        {
            UseDefaultClaimsAuthenticationManager = true;
            InheritHostClientIdentity = true;

            _manager = new WebSecurityTokenHandlerCollectionManager();
        }
    }
}
