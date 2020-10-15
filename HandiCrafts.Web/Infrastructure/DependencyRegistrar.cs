using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using HandiCrafts.Web.Infrastructure.Framework;
using HandiCrafts.Web.Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;


namespace HandiCrafts.Web.Infrastructure
{
    public class DependencyRegistrar
    {
        public void Register(IServiceCollection services)
        {
            // App
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddScoped<IAppContext, AppContext>();
            services.AddScoped<ScriptManager>();
            services.AddScoped<StyleManager>();
            services.AddScoped<UserManager>();
            services.AddScoped<SignInManager>();
            services.AddScoped<JwtTokenService>();
            services.AddSingleton<PartialViewResultExecutor>();
            
        }
    }
}
