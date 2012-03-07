using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Thinktecture.IdentityModel.Http
{
    public class RedirectBasedAuthenticationFixer : IHttpModule
    {
        public const string OverrideStatusCodeLabel = "RedirectBasedAuthenticationFixer::OverideStatusCodeLabel"; 
        public const string OverrideRedirectLabel = "RedirectBasedAuthenticationFixer::OverideRedirectLabel";

        public void Dispose()
        { }

        public void Init(HttpApplication context)
        {
            context.PostReleaseRequestState += OnPostReleaseRequestState;
            context.EndRequest += OnEndRequest;
        }

        private void OnPostReleaseRequestState(object source, EventArgs args)
        {
            var app = (HttpApplication)source;
            var response = app.Response;
            var request = app.Request;

            bool isAjax = request.Headers["X-Requested-With"] == "XMLHttpRequest";
            bool isExplicitOverride = app.Context.Items.Contains(OverrideRedirectLabel);

            if ((response.StatusCode == 401 || response.StatusCode == 403) && (isAjax || isExplicitOverride))
            {
                app.Context.Items[OverrideStatusCodeLabel] = response.StatusCode;
            }
        }

        private void OnEndRequest(object source, EventArgs args)
        {
            var app = (HttpApplication)source;
            var response = app.Response;

            if (app.Context.Items.Contains(OverrideStatusCodeLabel))
            {
                response.StatusCode = (int)app.Context.Items[OverrideStatusCodeLabel];
                response.RedirectLocation = null;
            }
        }
    }
}
