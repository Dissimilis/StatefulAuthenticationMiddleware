using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Principal;
using System.Text;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security.Infrastructure;
using Owin;

namespace OwinStatefulAuthentication
{
    public class StatefulAuthenticationMiddleware : AuthenticationMiddleware<StatefulAuthenticationOptions>
    {
        private ILogger _logger;

        public StatefulAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, StatefulAuthenticationOptions options)
            : base(next, options)
        {
            _logger = app.CreateLogger<StatefulAuthenticationHandler>();
        }

        protected override AuthenticationHandler<StatefulAuthenticationOptions> CreateHandler()
        {
            return new StatefulAuthenticationHandler(_logger);
        }
    }
}

