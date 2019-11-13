using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TicketSysteemMVC5.Startup))]
namespace TicketSysteemMVC5
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
