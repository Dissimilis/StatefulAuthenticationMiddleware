using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;

namespace OwinStatefulAuthentication
{
    public class StatefulAuthenticationHandler : AuthenticationHandler<StatefulAuthenticationOptions>
    {
        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            AuthenticationHeaderValue authorization = null;
            var authHeader = Request.Headers.Get("authorization");
            if (authHeader != null)
            {
                authorization = AuthenticationHeaderValue.Parse(authHeader);
            }

            if (authorization == null || string.IsNullOrWhiteSpace(authorization.Parameter))
            {
                return new AuthenticationTicket(null, null);
            }
            //else if (!authorization.Scheme.Equals(Options.AuthenticationType, StringComparison.OrdinalIgnoreCase))
            //{
            //    return new AuthenticationTicket(null, null);
            //}
            else
            {
                var identity = await Options.TokenProvider.GetIdentityAsync(authorization.Parameter, authorization.Scheme);
                if (identity == null)
                {
                    if (Options.OnInvalidToken != null)
                        Options.OnInvalidToken(authorization.Parameter);
                }
                var ticket = new AuthenticationTicket(identity, null);
                return ticket;
            }
        }
    }
}