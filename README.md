# OWIN stateful token authentication middleware
For when you want to store token in database

See WebApiDemo project for usage demo

**Usage examples:**
```csharp
//WebApi Startup class
public class Startup
{
    public void Configuration(IAppBuilder app)
    {
      var config = new HttpConfiguration();
      config.Routes.MapHttpRoute(
        name: "DefaultApi",
        routeTemplate: "api/{controller}/{id}",
        defaults: new { id = RouteParameter.Optional }
      ); 
      //there you can pass func for getting username from token
      //for more fine grained controll you can override StatefulTokenProvider.GetIdentityAsync
      app.UseStatefulAuthentication((token)=>Task.FromResult(token == "abcxyz123" ? "JohnSmith" : null));
      app.UseWebApi(config);
    }
}
```
