using System.IdentityModel.Tokens;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Web;

namespace Thinktecture.IdentityModel.Http
{
    public class AuthenticationHandler : DelegatingHandler
    {
        AuthenticationConfiguration _configuration;

        public AuthenticationHandler(AuthenticationConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
        /// In addition, the handler tries to authenticate the client if a credential is present
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>. The task object representing the asynchronous operation.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="request"/> was null.</exception>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                TryAuthenticateClient(request);
            }
            catch (SecurityTokenValidationException)
            {
                return Task<HttpResponseMessage>.Factory.StartNew(() =>
                {
                    var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    SetAuthenticateHeader(response);

                    return response;
                });
            }

            return base.SendAsync(request, cancellationToken).ContinueWith(
                (task) =>
                {
                    var response = task.Result;

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        SetAuthenticateHeader(response);    
                    }

                    return response;
                });
        }

        private void TryAuthenticateClient(HttpRequestMessage request)
        {
            var header = request.Headers.Authorization;

            // if no authorization header is present, 
            // we take whatever has been set on the request and turn it into a claims principal
            // (that could be an anonymous principal)
            if (header == null)
            {
                if (!_configuration.InheritHostClientIdentity ||
                    request.GetUserPrincipal() == null)
                {
                    SetPrincipal(ClaimsPrincipal.AnonymousPrincipal, request);
                    return;
                }

                var cp = ClaimsPrincipal.CreateFromPrincipal(request.GetUserPrincipal());
                SetPrincipal(cp, request);
                return;
            }

            // authorization header is present
            // try to validate the credential, otherwise 401
            IClaimsPrincipal principal;
            try
            {
                principal = _configuration.Handler.ValidateWebToken(header.Scheme, header.Parameter);

                if (principal == null)
                {
                    throw new SecurityTokenValidationException();
                }
            }
            catch
            {
                throw new SecurityTokenValidationException();
            }

            // do claims transformation
            if (_configuration.ClaimsAuthenticationManager != null)
            {
                principal = _configuration.ClaimsAuthenticationManager.Authenticate(request.RequestUri.AbsoluteUri, principal);
            }
            else if (_configuration.UseDefaultClaimsAuthenticationManager)
            {
                principal = FederatedAuthentication.ServiceConfiguration.ClaimsAuthenticationManager.Authenticate(request.RequestUri.AbsoluteUri, principal);
            }

            SetPrincipal(principal, request);
        }

        /// <summary>
        /// Sets the claims principal on Thread.CurrentPrincipal and the request message
        /// </summary>
        /// <param name="The Principal"></param>
        /// <param name="The current request message"></param>
        private void SetPrincipal(IClaimsPrincipal principal, HttpRequestMessage request)
        {
            Thread.CurrentPrincipal = principal;
            request.Properties[HttpPropertyKeys.UserPrincipalKey] = principal;
        }

        private void SetAuthenticateHeader(HttpResponseMessage response)
        {
            response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(_configuration.DefaultAuthenticationScheme));
        }
    }
}
