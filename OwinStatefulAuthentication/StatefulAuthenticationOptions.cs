using System;
using Microsoft.Owin.Security;

namespace OwinStatefulAuthentication
{
    /// <summary>
    /// Options which control the processing of the authorization header
    /// </summary>
    public class StatefulAuthenticationOptions : AuthenticationOptions
    {
        public StatefulTokenProvider TokenProvider { get; private set; }

        /// <summary>
        /// Called when repository returns not user 
        /// </summary>
        public Action<string> OnInvalidToken { get; set; }

        public StatefulAuthenticationOptions(StatefulTokenProvider tokenProvider) : base("Bearer")
        {
            
            if (tokenProvider == null)
                throw new ArgumentNullException("repository");
            TokenProvider = tokenProvider;
        }
    }
}