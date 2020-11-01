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

            services.AddMemoryCache();

            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization()
                .AddJsonOptions(option => option.JsonSerializerOptions.PropertyNamingPolicy = null);

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
                .AddCookie(options => {
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
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            /*UseAthentication(app);*/
            app.UseAuthentication();

            //if (bool.Parse(_appConfiguration["Authentication:JwtBearer:IsEnabled"]))
            //{
            //    app.UseJwtTokenMiddleware();
            //}

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        #region Authentication

        //private SymmetricSecurityKey signingKey;

        //private void AddAuthentication(IServiceCollection services)
        //{
        //    services.AddAuthentication()
        //        .AddCookie()
        //        .AddJwtBearer(options =>
        //        {
        //            options.TokenValidationParameters = new TokenValidationParameters
        //            {
        //                ValidateIssuerSigningKey = true,
        //                IssuerSigningKey = GetSigningKey(),
        //                ValidateIssuer = true,
        //                ValidIssuer = "http://nikan.com",
        //                ValidateAudience = true,
        //                ValidAudience = "api-users",
        //                ValidateLifetime = false,
        //                ClockSkew = TimeSpan.Zero
        //            };
        //        });

        //    services.ConfigureApplicationCookie(options =>
        //    {
        //        options.LoginPath = new PathString("/account/login");
        //        options.AccessDeniedPath = new PathString("/account/login");
        //        options.LogoutPath = new PathString("/account/logout");
        //        options.Cookie.Name = "HandiCrafts.WebCookie"; //change this name for every project
        //});
        //}

        //private void UseAthentication(IApplicationBuilder app)
        //{
        //    #region Token Provider

        //    /*async Task<ClaimsIdentity> GetIdentity(string username, string password)
        //    {
        //        //if we try to login after changing password,"CheckPasswordAsync" method returns false ,because EFCore caches data.(if we reastart iis it will work!!!)
        //        //so we need new instance of Dbcontext and create new instance of Usermanager on the new created context
        //        using (var serviceScope =
        //            app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
        //        {
        //            var um = serviceScope.ServiceProvider.GetService<UserManager>();
        //            var rm = serviceScope.ServiceProvider.GetService<RoleManager<Role>>();
        //            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        //            {
        //                var user = await um.FindByNameAsync(username);
        //                if (user != null)
        //                {
        //                    var valid = await um.CheckPasswordAsync(user, password);
        //                    if (valid)
        //                    {
        //                        if (!user.IsActive)
        //                            throw new Exception("Your account has been deactivated.");

        //                        var jwtGenerator = new JwtTokenService(um, rm);
        //                        var claimIdentity = await jwtGenerator.GetUserClaims(user);
        //                        return claimIdentity;
        //                    }
        //                }
        //            }
        //        }

        //        // Credentials are invalid, or account doesn't exist
        //        return null;
        //    }

        //    app.UseSimpleTokenProvider(new TokenProviderOptions
        //    {
        //        Path = "/api/token",
        //        Audience = "api-users",
        //        Issuer = "http://fhfghfh.gdfg",
        //        Expiration = TimeSpan.FromDays(365),
        //        SigningCredentials = new SigningCredentials(GetSigningKey(), SecurityAlgorithms.HmacSha256),
        //        IdentityResolver = GetIdentity,
        //    });*/

        //    #endregion

        //    app.UseAuthentication();
        //}

        //private SymmetricSecurityKey GetSigningKey()
        //{
        //    if (signingKey != null)
        //    {
        //        return signingKey;
        //    }

        //    var secretKey = Configuration.AsEnumerable().FirstOrDefault(x => x.Key == "AppConfig:JwtSecretKey").Value;
        //    if (!string.IsNullOrEmpty(secretKey))
        //    {
        //        signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
        //        return signingKey;
        //    }

        //    throw new Exception("SecretKey is not available");
        //}

        #endregion
    }
}
