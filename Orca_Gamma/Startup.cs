using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Orca_Gamma.Startup))]
namespace Orca_Gamma
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
