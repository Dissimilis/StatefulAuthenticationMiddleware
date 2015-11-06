using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;

namespace WebApiDemo
{
    class Program
    {

        static void Main(string[] args)
        {
            //setup fake users database
            MemoryCache.Default.Add("TokenXXX", new User("Marius", "marius@example.org") { FullName = "Marius V" }, DateTimeOffset.MaxValue);
            MemoryCache.Default.Add("TokenYYY", new User("Marius", "marius@example.org") { FullName = "Marius V" }, DateTimeOffset.MaxValue);
            MemoryCache.Default.Add("TokenZZZ", new User("Tomas", "tomas@example.org") {FullName = "Tomas Spokas"}, DateTimeOffset.MaxValue);

            var listenUrl = "http://localhost:8080";

            WebApp.Start<Startup>(new StartOptions(listenUrl));

            Console.WriteLine("Call {0}/api/Guest or {0}/api/Auth (with correct token)", listenUrl);
            Console.ReadKey();
        }
        
        
        /// <summary>
        /// Method used for getting unique username based on token;
        /// Roles can be implemented by making custom StatefulTokenProvider.GetIdentityAsync and setting claims with ClaimTypes.Role
        /// </summary>
        public static async Task<string> GetUser(string token)
        {
            await Task.Delay(100); //simulate some database delay
            var user = MemoryCache.Default.Get(token) as User;
            if (user != null)
            {
                return user.Username;
            }
            return null;
        }
    }
}
