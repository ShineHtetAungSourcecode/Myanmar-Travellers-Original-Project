using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyanmarTravellers.Startup))]
namespace MyanmarTravellers
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
