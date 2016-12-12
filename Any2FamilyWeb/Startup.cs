using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Any2FamilyWeb.Startup))]
namespace Any2FamilyWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
