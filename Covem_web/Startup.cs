using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Covem_web.Startup))]
namespace Covem_web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
