using System;
using System.Threading.Tasks;
using Owin;

namespace OwinStatefulAuthentication
{
    public static class StatefulAuthenticationExtensions
    {
        /// <summary>
        /// Adds stateful token processing to OWIN application pipeline; Options with token repository must be set.
        /// </summary>
        /// <param name="options">Options which control the processing of the bearer header</param>
        public static IAppBuilder UseStatefulAuthentication(this IAppBuilder app, StatefulAuthenticationOptions options)
        {
            if (app == null)
                throw new ArgumentNullException("app");
            if (options == null)
                throw new ArgumentNullException("options");
            app.Use(typeof(StatefulAuthenticationMiddleware), app, options);
            return app;
        }

        /// <summary>
        /// Adds stateful token processing to OWIN application pipeline; Options with token repository must be set.
        /// </summary>
        /// <param name="userFromToken">Async function to retrieve unique username from token</param>
        public static IAppBuilder UseStatefulAuthentication(this IAppBuilder app, Func<string, Task<string>> userFromToken)
        {
            if (app == null)
                throw new ArgumentNullException("app");
            if (userFromToken == null)
                throw new ArgumentNullException("userFromToken");

            var repo = new StatefulTokenProvider(userFromToken);
            var options = new StatefulAuthenticationOptions(repo);
            app.Use(typeof(StatefulAuthenticationMiddleware), app, options);
            return app;
        }
    }
}