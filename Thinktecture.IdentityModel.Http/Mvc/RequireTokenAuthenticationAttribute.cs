/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.Web;
using System.Web.Mvc;
using Microsoft.IdentityModel.Web;
using WIF = Microsoft.IdentityModel.Claims;

namespace Thinktecture.IdentityModel.Mvc
{
    public class RequireTokenAuthenticationAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.User.Identity.IsAuthenticated &&
                httpContext.User.Identity.AuthenticationType.Equals(WIF.AuthenticationTypes.Federation, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // do the redirect to the STS
            var message = FederatedAuthentication.WSFederationAuthenticationModule.CreateSignInRequest("passive", filterContext.HttpContext.Request.RawUrl, rememberMeSet: false);
            filterContext.Result = new RedirectResult(message.RequestUrl);
        }
    }
}