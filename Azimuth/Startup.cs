using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Azimuth.Startup))]
namespace Azimuth
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
