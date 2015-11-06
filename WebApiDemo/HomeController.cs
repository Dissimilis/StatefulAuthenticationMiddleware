using System.Web.Http;

namespace WebApiDemo
{
    public class GuestController : ApiController
    {
        public string Get()
        {
            return "Hello guest";
        }
    }  
    public class AuthController : ApiController
    {
        [Authorize]
        public string Get()
        {
            return "Hello "+User.Identity.Name;
        }
    }
}