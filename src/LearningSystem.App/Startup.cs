using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LearningSystem.App.Startup))]
namespace LearningSystem.App
{
    public partial class Startup 
    {
        public void Configuration(IAppBuilder app) 
        {
            ConfigureAuth(app);
        }
    }
}
