using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace OwinStatefulAuthentication
{
    /// <summary>
    /// Repository for getting identity based on token
    /// Override GetIdentityAsync if you want to controll ClaimsIdentity creation (i.e. for setting roles)
    /// </summary>
    public class StatefulTokenProvider
    {
        private readonly Func<string, Task<string>> _userFromToken;

        /// <param name="userFromTokenAsync">Async function for getting unique user name</param>
        public StatefulTokenProvider(Func<string, Task<string>> userFromTokenAsync)
        {
            if (userFromTokenAsync == null)
                throw new ArgumentNullException("userFromTokenAsync");
            _userFromToken = userFromTokenAsync;
        }

        /// <param name="userFromTokenAsync">Function for getting unique user name</param>
        public StatefulTokenProvider(Func<string, string> userFromToken)
        {
            if (userFromToken == null)
                throw new ArgumentNullException("userFromToken");
            _userFromToken = (t) => Task.FromResult(userFromToken(t));
        }

        /// <summary>
        /// Generates ClaimsIdentity from provided func
        /// </summary>
        /// <param name="scheme">Authorization scheme from HTTP header</param>
        /// <returns>ClaimsIdentity with name set from func result</returns>
        public virtual async Task<ClaimsIdentity> GetIdentityAsync(string token, string scheme)
        {
            if (string.IsNullOrEmpty(token) || token.Length > 4000) //dont call repository when token is too large
                return null;
            var identityName = await _userFromToken(token);
            if (!String.IsNullOrEmpty(identityName))
            {
                var claims = new[] {new Claim(ClaimTypes.Name, identityName)};
                var identity = new ClaimsIdentity(claims, scheme) { BootstrapContext = token };
                
                return identity;
            }
            return null;
        }

        /// <summary>
        /// Generates secure random token for sending to client
        /// </summary>
        /// <param name="bytesLength">Length of token (not the resulting string); Must be between 32 and 2048</param>
        /// <returns>Base64 encoded random data</returns>
        public static string GenerateToken(int bytesLength = 32)
        {
            if (bytesLength < 32 || bytesLength > 2048)
                throw new ArgumentException("Range is invalid. Must be between 32 and 2048","bytesLength");
            var tokenData = new byte[bytesLength];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(tokenData);
            }
            string token = Convert.ToBase64String(tokenData);
            return token;
        }
    }
}