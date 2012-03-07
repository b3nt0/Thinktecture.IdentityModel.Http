﻿using System.Web;
using Microsoft.IdentityModel.Web;

namespace Thinktecture.IdentityModel.Web
{
    class ServiceAwareWSFederationAuthenticationModule : WSFederationAuthenticationModule
    {
        public const string DefaultLabel = "ServiceAwareWSFederationAuthenticationModule:IsService";

        protected override void OnAuthorizationFailed(AuthorizationFailedEventArgs e)
        {
            base.OnAuthorizationFailed(e);

            var isService = HttpContext.Current.Items[DefaultLabel];

            if (isService != null)
            {
                e.RedirectToIdentityProvider = false;
            }
        }
    }
}
