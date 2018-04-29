using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(apiparttwo.Startup))]
namespace apiparttwo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
