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

            services.AddRazorPages();

            //AddAuthentication(services);



            new DependencyRegistrar().Register(services);




            services.AddAuthentication(configureOptions =>
            {
                configureOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    //IssuerSigningKey = new SymmetricSecurityKey(key),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["BearerTokens:Key"])),
                    ValidateIssuer = true,
                    //ValidIssuer = issuer, 
                    ///ValidIssuer = Configuration["BearerTokens:Issuer"],
                    ///ValidateAudience = true,
                    //ValidAudience = audience,
                    ///ValidAudience = Configuration["BearerTokens:Audience"],
                    ValidateLifetime = true,

                    ValidIssuer = "http://nikan.com",
                    ValidateAudience = true,
                    ValidAudience = "api-users",
                };
            
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["your-cookie"];
                        return Task.CompletedTask;
                    }
                };


                services.ConfigureApplicationCookie(options =>
                {
                    options.LoginPath = new PathString("/account/login");
                    options.AccessDeniedPath = new PathString("/account/login");
                    options.LogoutPath = new PathString("/account/logout");
                    options.Cookie.Name = "HandiCrafts.WebCookie"; //change this name for every project
                });
            });



            // Auth
            //services.Configure<BearerTokensOptions>(options => Configuration.GetSection("BearerTokens").Bind(options));

            /*services
                .AddAuthentication(options =>
                {
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = Configuration["BearerTokens:Issuer"],
                        ValidAudience = Configuration["BearerTokens:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["BearerTokens:Key"])),
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                    cfg.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                            logger.LogError("Authentication failed.", context.Exception);
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            var tokenValidatorService = context.HttpContext.RequestServices.GetRequiredService<ITokenValidatorService>();
                            return tokenValidatorService.ValidateAsync(context);
                        },
                        OnMessageReceived = context =>
                        {
                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                            logger.LogError("OnChallenge error", context.Error, context.ErrorDescription);
                            return Task.CompletedTask;
                        }
                    };
                });*/
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
        //        Issuer = "http://nikan.com",
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
