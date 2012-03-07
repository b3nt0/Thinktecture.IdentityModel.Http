﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Text;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Tokens;
using Thinktecture.IdentityModel.Claims;

namespace Thinktecture.IdentityModel.Http
{
    /// <summary>
    /// Generic security token handler for username/password type credentials
    /// </summary>
    public class WebUserNameSecurityTokenHandler : UserNameSecurityTokenHandler, IWebSecurityTokenHandler
    {
        public IClaimsPrincipal ValidateWebToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException("token");
            }

            var decoded = DecodeBasicAuthenticationHeader(token);
            var securityToken = new UserNameSecurityToken(decoded.Item1, decoded.Item2);

            return ClaimsPrincipal.CreateFromIdentities(ValidateToken(securityToken));
        }

        protected virtual Tuple<string, string> DecodeBasicAuthenticationHeader(string basicAuthToken)
        {
            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            string userPass = encoding.GetString(Convert.FromBase64String(basicAuthToken));
            int separator = userPass.IndexOf(':');

            var credential = new Tuple<string, string>(
                userPass.Substring(0, separator),
                userPass.Substring(separator + 1));

            return credential;
        }

        /// <summary>
        /// Callback type for validating the credential
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>True when the credential could be validated succesfully. Otherwise false.</returns>
        public delegate bool ValidateUserNameCredentialDelegate(string username, string password);

        /// <summary>
        /// Gets or sets the credential validation callback
        /// </summary>
        /// <value>
        /// The credential validation callback.
        /// </value>
        public ValidateUserNameCredentialDelegate ValidateUserNameCredential { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebUserNameSecurityTokenHandler"/> class.
        /// </summary>
        public WebUserNameSecurityTokenHandler()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebUserNameSecurityTokenHandler"/> class.
        /// </summary>
        /// <param name="validateUserNameCredential">The credential validation callback.</param>
        public WebUserNameSecurityTokenHandler(ValidateUserNameCredentialDelegate validateUserNameCredential)
        {
            if (validateUserNameCredential == null)
            {
                throw new ArgumentNullException("ValidateUserNameCredential");
            }

            ValidateUserNameCredential = validateUserNameCredential;
        }

        /// <summary>
        /// Validates the user name credential core.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        protected virtual bool ValidateUserNameCredentialCore(string userName, string password)
        {
            if (ValidateUserNameCredential == null)
            {
                throw new InvalidOperationException("ValidateUserNameCredentialDelegate not set");
            }

            return ValidateUserNameCredential(userName, password);
        }

        /// <summary>
        /// Validates the username and password.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>A ClaimsIdentityCollection representing the identity in the token</returns>
        public override ClaimsIdentityCollection ValidateToken(SecurityToken token)
        {
            if (token == null)
            {
                throw new ArgumentNullException("token");
            }

            if (base.Configuration == null)
            {
                throw new InvalidOperationException("No Configuration set");
            }

            UserNameSecurityToken unToken = token as UserNameSecurityToken;
            if (unToken == null)
            {
                throw new ArgumentException("SecurityToken is no UserNameSecurityToken");
            }

            if (!ValidateUserNameCredentialCore(unToken.UserName, unToken.Password))
            {
                throw new SecurityTokenValidationException(unToken.UserName);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, unToken.UserName),
                new Claim(ClaimTypes.AuthenticationMethod, AuthenticationMethods.Password),
                AuthenticationInstantClaim.Now
            };

            var identity = new ClaimsIdentity(claims, "Basic");

            if (base.Configuration.SaveBootstrapTokens)
            {
                if (this.RetainPassword)
                {
                    identity.BootstrapToken = unToken;
                }
                else
                {
                    identity.BootstrapToken = new UserNameSecurityToken(unToken.UserName, null);
                }
            }

            return new ClaimsIdentityCollection(new IClaimsIdentity[] { identity });
        }

        /// <summary>
        /// Gets a value indicating whether this instance can validate a token.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can validate a token; otherwise, <c>false</c>.
        /// </value>
        public override bool CanValidateToken
        {
            get
            {
                return true;
            }
        }
    }
}
