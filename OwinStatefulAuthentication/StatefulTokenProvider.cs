using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace OwinStatefulAuthentication
{
    /// <summary>
    /// Repository for getting identity based on token
    /// Override GetIdentityAsync if you want to controll ClaimsIdentity creation
    /// </summary>
    public class StatefulTokenProvider
    {
        private readonly Func<string, Task<string>> _userFromToken;
        private readonly Func<string, Task<UserWithRoles>> _userWithRolesFromToken;



        protected StatefulTokenProvider() { }


        /// <param name="userFromTokenAsync">Async function for getting unique user name</param>
        public StatefulTokenProvider(Func<string, Task<string>> userFromTokenAsync)
        {
            if (userFromTokenAsync == null)
                throw new ArgumentNullException("userFromTokenAsync");
            _userFromToken = userFromTokenAsync;
        }

        /// <param name="userFromToken">Function for getting unique user name</param>
        public StatefulTokenProvider(Func<string, string> userFromToken)
        {
            if (userFromToken == null)
                throw new ArgumentNullException("userFromToken");
            _userFromToken = (t) => Task.FromResult(userFromToken(t));
        }

        /// <param name="userFromTokenAsync">Async function for getting unique user with roles (roles will be set as ClaimType.Role)</param>
        public StatefulTokenProvider(Func<string, Task<UserWithRoles>> userFromTokenAsync)
        {
            if (userFromTokenAsync == null)
                throw new ArgumentNullException("userFromTokenAsync");
            _userWithRolesFromToken = userFromTokenAsync;
        }

        /// <param name="userFromToken">Function for getting unique user with roles (roles will be set as ClaimType.Role)</param>
        public StatefulTokenProvider(Func<string, UserWithRoles> userFromToken)
        {
            if (userFromToken == null)
                throw new ArgumentNullException("userFromToken");
            _userWithRolesFromToken = (t) => Task.FromResult(userFromToken(t));
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
            if (_userFromToken != null)
            {
                var identityName = await _userFromToken(token);
                if (!String.IsNullOrEmpty(identityName))
                {
                    var claims = new[] {new Claim(ClaimTypes.Name, identityName)};
                    var identity = new ClaimsIdentity(claims, scheme) {BootstrapContext = token};

                    return identity;
                }
            }
            else //with roles
            {
                var user = await _userWithRolesFromToken(token);
                if (user != null && !String.IsNullOrEmpty(user.UserName))
                {
                    var claims = new List<Claim>(12) {new Claim(ClaimTypes.Name, user.UserName)};
                    if (user.Roles != null)
                        claims.AddRange(user.Roles.Select(r => new Claim(ClaimTypes.Role, r)));
                    var identity = new ClaimsIdentity(claims, scheme) { BootstrapContext = token };
                    return identity;
                }
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

    public class UserWithRoles
    {
        public string UserName { get; set; }
        public IEnumerable<string> Roles { get; set; }
        
        public UserWithRoles (string username, IEnumerable<string> roles = null)
        {
            UserName = username;
            Roles = roles;
        }
    }
}