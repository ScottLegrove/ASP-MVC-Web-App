using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SPM.Startup))]
namespace SPM
{
    public partial class Startup
    {    
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
