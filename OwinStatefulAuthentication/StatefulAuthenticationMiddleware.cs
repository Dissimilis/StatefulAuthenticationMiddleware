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
        public StatefulAuthenticationMiddleware(OwinMiddleware next,IAppBuilder app, StatefulAuthenticationOptions options) : base(next, options)
        {

        }

        protected override AuthenticationHandler<StatefulAuthenticationOptions> CreateHandler()
        {
            return new StatefulAuthenticationHandler();
        }
    }
}

