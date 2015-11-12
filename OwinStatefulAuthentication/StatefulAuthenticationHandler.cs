using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;

namespace OwinStatefulAuthentication
{
    public class StatefulAuthenticationHandler : AuthenticationHandler<StatefulAuthenticationOptions>
    {
        private ILogger _logger;

        public StatefulAuthenticationHandler(ILogger logger)
        {
            _logger = logger;
        }

        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            try
            {
                AuthenticationHeaderValue authorization = null;
                var authHeader = Request.Headers.Get("authorization");
                if (authHeader != null)
                {
                    authorization = AuthenticationHeaderValue.Parse(authHeader);
                }

                if (authorization != null && string.IsNullOrWhiteSpace(authorization.Parameter))
                {
                    ClaimsIdentity identity = null;
                    try
                    {
                        identity = await Options.TokenProvider.GetIdentityAsync(authorization.Parameter, authorization.Scheme);
                        if (identity == null)
                        {
                            if (Options.OnInvalidToken != null)
                                Options.OnInvalidToken(authorization.Parameter);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (_logger != null)
                            _logger.WriteError("Error calling user method in authentication core", ex);
                    }
                    var ticket = new AuthenticationTicket(identity, null);
                    return ticket;

                }
            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.WriteError("Error in authentication core", ex);
            }

            return new AuthenticationTicket(null, null);
        }
    }
}