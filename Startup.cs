using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(eStoreJA.Startup))]
namespace eStoreJA
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
