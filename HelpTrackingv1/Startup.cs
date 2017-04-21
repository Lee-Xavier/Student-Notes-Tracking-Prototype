using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HelpTrackingv1.Startup))]
namespace HelpTrackingv1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
