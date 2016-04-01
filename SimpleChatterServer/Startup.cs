using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SimpleChatterServer.Startup))]
namespace SimpleChatterServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}