using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PayTransApi.Startup))]
namespace PayTransApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
