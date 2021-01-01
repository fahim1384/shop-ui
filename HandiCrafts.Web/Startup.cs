using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
//using HandiCrafts.Web.Infrastructure.Security;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
//
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore.Internal;
using HandiCrafts.Web.Infrastructure;
using HandiCrafts.Core.Domain.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AutoMapper;
using SmartBreadcrumbs.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Razor;
using HandiCrafts.Web.Infrastructure.Security;
//using HandiCrafts.Web.BpService;

namespace HandiCrafts.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //services.Configure<AppConfig>(options => Configuration.GetSection("AppConfig").Bind(options));

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


            /*services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
                .AddDefaultTokenProviders();*/

            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                T Conf<T>(T cult) where T : CultureInfo
                {
                    cult.NumberFormat.NumberDecimalSeparator = ".";
                    cult.NumberFormat.NumberGroupSeparator = " ";
                    cult.NumberFormat.CurrencyDecimalSeparator = ".";
                    cult.DateTimeFormat.AMDesignator = "AM";
                    cult.DateTimeFormat.PMDesignator = "PM";
                    cult.DateTimeFormat.FullDateTimePattern = "yyyy/MM/dd HH:mm:ss.fff";

                    return cult;
                }

                var supportedCultures = new List<CultureInfo>
                {
                    Conf(new CultureInfo("fa-IR")),
                    //Conf(new CultureInfo("fa")),
                    //Conf(new CultureInfo("az-Latn-AZ")),
                    //Conf(new CultureInfo("az"))
                    Conf(new CultureInfo("tr-TR")),
                    //Conf(new CultureInfo("tr")),
                    //Conf(new CultureInfo("en-US")),
                    //Conf(new CultureInfo("en-GB")),
                    //Conf(new CultureInfo("en")),
                };

                options.DefaultRequestCulture = new RequestCulture("fa-IR", "fa-IR");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders = new List<IRequestCultureProvider>()
                {
                    new QueryStringRequestCultureProvider(),
                    new CookieRequestCultureProvider()
                };
            });

            services.AddMemoryCache();

            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                        factory.Create(typeof(SharedResource));
                })
                .AddJsonOptions(option => option.JsonSerializerOptions.PropertyNamingPolicy = null);

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            //settings.DefaultValueHandling = DefaultValueHandling.Include;

            services.AddRazorPages().AddNewtonsoftJson();

            services.AddSession();

            //AddAuthentication(services);

            new DependencyRegistrar().Register(services);

            services.AddBreadcrumbs(GetType().Assembly, options =>
            {
                options.TagName = "nav";
                options.TagClasses = "";
                options.OlClasses = "breadcrumb";
                options.LiClasses = "breadcrumb-item";
                options.ActiveLiClasses = "breadcrumb-item active";
                options.SeparatorElement = "<li class=\"separator\">/</li>";
            });

            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            var httpContextAccessor = new HttpContextAccessor();
            services.AddHttpClient("myHttpClient", options =>
            {
                options.BaseAddress = new Uri(Configuration.GetValue<string>("BaseUrl"));

                var identity = httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    // or
                    if (claims.ToList().Count() > 0)
                    {
                        var token = identity.Claims.First(o => o.Type.Contains("nameidentifier")).Value;
                        options.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }
                }

                /*var token = httpContextAccessor.HttpContext.Session.GetString("token");
                if (!string.IsNullOrEmpty(token))
                {
                    options.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", httpContextAccessor.HttpContext.Session.GetString("token"));
                }*/

                //...
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login/";
                    options.AccessDeniedPath = "/Account/Login/";
                    options.ExpireTimeSpan = TimeSpan.FromDays(1);
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        //IssuerSigningKey = GetSigningKey(),
                        ValidateIssuer = true,
                        //ValidIssuer = "http://test.com",
                        ValidateAudience = true,
                        ValidAudience = "api-users",
                        ValidateLifetime = false,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/account/login");
                options.AccessDeniedPath = new PathString("/account/login");
                options.LogoutPath = new PathString("/account/logout");
                options.Cookie.Name = "HandiCrafts.WebCookie";
                options.ExpireTimeSpan = TimeSpan.FromDays(1);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<RequestLocalizationOptions> localizationOptions)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseSession();

            app.UseRouting();

            #region Localization

            app.UseRequestLocalization(localizationOptions.Value);
            /*IList<CultureInfo> supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en-US"),
                new CultureInfo("fa-IR"),
            };

            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("fa-IR", "fa-IR"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
                RequestCultureProviders = new List<IRequestCultureProvider>()
                {
                    new QueryStringRequestCultureProvider(),
                    new CookieRequestCultureProvider()
                }
            };
            app.UseRequestLocalization(options);*/

            #endregion

            /*UseAthentication(app);*/
            app.UseAuthentication();

            //if (bool.Parse(_appConfiguration["Authentication:JwtBearer:IsEnabled"]))
            //{
            //    app.UseJwtTokenMiddleware();
            //}

            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                try
                {
                    await next();

                    if (context.Response.StatusCode == 404)
                    {
                        context.Response.Redirect("/404");
                    }

                    if (context.Response.StatusCode == 401)
                    {
                        if (context.Request.Path.StartsWithSegments("seller"))
                            context.Response.Redirect("/seller/Account/login");
                        else
                            context.Response.Redirect("/Account/login");
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("401"))
                    {
                        context.Response.Redirect("/Account/Logout");
                    }
                }
            });

            app.UseErrorHandleMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "area",
                    pattern: "{area:exists}/{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                /*endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area}/{controller}/{did?}/{action=Index}/{id?}");*/
            });

            //app.Run(context =>
            //{
            //    context.Response.StatusCode = 404;
            //    return Task.FromResult(0);
            //});
        }


    }
}
