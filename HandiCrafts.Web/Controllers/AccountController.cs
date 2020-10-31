using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using HandiCrafts.Core.Domain.Users;
using HandiCrafts.Web.Interfaces;
using HandiCrafts.Web.Models;
using HandiCrafts.Web.Models.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HandiCrafts.Web.Controllers
{
    public class AccountController : BasePublicController
    {

        #region Fields

        IHttpClientFactory _httpClientFactory;

        #endregion

        #region Constructors

        public AccountController(
            IHttpClientFactory httpClientFactory,
            ILogger<AccountController> logger,
            IStringLocalizer<SharedResource> localizer,
            IMapper mapper) : base(logger, localizer, mapper)
        {
            _httpClientFactory = httpClientFactory;
        }


        #endregion

        #region Actions


        public IActionResult Login(string returnUrl = null)
        {

            if (!Url.IsLocalUrl(returnUrl))
            {
                returnUrl = null;
            }

            return View("LoginContainer", new LoginContainerModel
            {
                /*LoginModel = new LoginModel
                {
                    ReturnUrl = returnUrl,
                    //ReCaptchaKey = _configuration["GoogleReCaptcha:key"]//https://codepen.io/api521/pen/NVqbNY
                },
                RegisterModel = new RegisterModel
                {
                    //ReCaptchaKey = _configuration["GoogleReCaptcha:key"]
                },*/

                ShortLoginModel = new ShortLoginModel() { },

                DefaultTab = "login"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<ResponseState<LoginRegisterDto>> Login(ShortLoginModel model)
        {
            return TryCatch(async () =>
            {

                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                CustomerLoginRegisterClient client = new CustomerLoginRegisterClient(BaseUrl, httpClient);

                string email = null;
                long? mobile = null;

                string EmailorMobile = model.EmailorMobileNo.ToString();
                if (Regex.IsMatch(EmailorMobile, @"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", RegexOptions.IgnoreCase))
                {
                    email = EmailorMobile;
                }
                else if (Regex.IsMatch(EmailorMobile, @"09(0[1-9]|[0-9][0-9]|3[0-9]|2[0-1])-?[0-9]{3}-?[0-9]{4}", RegexOptions.IgnoreCase))
                {
                    mobile = long.Parse(EmailorMobile);
                }

                var result = await client.UIAsync(email, mobile);

                return Success(data: result.Obj, message: result.ResultMessage);

                /*if (model.Username != "148272579" || model.Password != "123456")
                {
                    return Error<string>(null, "مشخصات وارد شده صحیح نمی باشد");
                }


                if (!string.IsNullOrEmpty(model.ReturnUrl))
                {
                    return Success<string>(model.ReturnUrl);
                }
                else
                {
                    return Success<string>("/user");
                }*/

            });

        }

        //public IActionResult _LoginUP()
        //{
        //    return View();
        //}

        /*[HttpPost]
        public Task<DefaultResponseState> _LoginUP(LoginModel model)
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                CustomerLoginRegisterClient client = new CustomerLoginRegisterClient(BaseUrl, httpClient);

                var result = await client.UIAsync(new UserLoginModel()
                {
                    Username = model.Username,
                    Password = model.Password
                });

                if (result.ResultCode == 200)
                {
                    var claims = new[] {
                        new Claim("token", result.Obj.Token),
                        new Claim("fullname", result.Obj.Fullname),
                        //new Claim(JwtRegisteredClaimNames.Exp, "1"),
                    };

                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MyNameIsEsmaeilVahedi"));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(

                        //issuer: "https://www.tabrizcraft.ir",
                        //audience: "https://www.tabrizcraft.ir",
                        expires: DateTime.Now.AddHours(3),
                        signingCredentials: credentials,
                        claims: claims
                        );

                    new JwtSecurityTokenHandler().WriteToken(token);

                    //
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, result.Obj.Token));
                    identity.AddClaim(new Claim(ClaimTypes.Name, result.Obj.Fullname));
                    identity.AddClaim(new Claim(ClaimTypes.Role, "User"));

                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = true });
                    //

                    return Success(message: result.ResultMessage);
                }
                else
                {
                    return Error(message: result.ResultMessage);
                }
            });

        }*/

        public IActionResult FastRegister(long userid)
        {
            return View("FastRegister", new ShortRegisterModel()
            {
                UserId = userid
            });
        }

        [HttpPost]
        public Task<ResponseState<LoginResultDtoSingleResult>> FastRegister(ShortRegisterModel model)
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                ByPassClient client = new ByPassClient(BaseUrl, httpClient);

                var result = await client.UIAsync(model.UserId, model.Password);

                if (result.ResultCode != 200)
                    return Error<LoginResultDtoSingleResult>(null, result.ResultMessage);

                var claims = new[] {
                        new Claim("token", result.Obj.Token),
                        new Claim("fullname", result.Obj.Fullname),
                        //new Claim(JwtRegisteredClaimNames.Exp, "1"),
                    };

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MyNameIsEsmaeilVahedi"));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(

                    //issuer: "https://www.tabrizcraft.ir",
                    //audience: "https://www.tabrizcraft.ir",
                    expires: DateTime.Now.AddHours(3),
                    signingCredentials: credentials,
                    claims: claims
                    );

                new JwtSecurityTokenHandler().WriteToken(token);

                //
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, result.Obj.Token));
                identity.AddClaim(new Claim(ClaimTypes.Name, result.Obj.Fullname));
                identity.AddClaim(new Claim(ClaimTypes.Role, "User"));

                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = true });
                //

                return Success(data: result, message: result.ResultMessage);

            });
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Account/Login");
        }

        public IActionResult VerifyCode(long? userid)
        {
            if(userid == null)
            {
                return View("LoginContainer", new LoginContainerModel
                {
                    RegisterModel = new RegisterModel
                    {
                    },
                    DefaultTab = "login"
                });
            }

            return View("LoginContainer", new LoginContainerModel
            {
                RegisterModel = new RegisterModel
                {
                },
                DefaultTab = "verifyCode"
            });
        }

        [HttpPost]
        public Task<ResponseState<LoginResultDtoSingleResult>> VerifyCode(AcceptCodeModel model)
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                ByActivationCodeClient client = new ByActivationCodeClient(BaseUrl, httpClient);

                var result = await client.UIAsync(model.UserId, model.AcceptCode);

                if (result.ResultCode != 200)
                    return Error<LoginResultDtoSingleResult>(null, result.ResultMessage);

                var claims = new[] {
                        new Claim("token", result.Obj.Token),
                        new Claim("fullname", result.Obj.Fullname),
                        //new Claim(JwtRegisteredClaimNames.Exp, "1"),
                    };

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MyNameIsEsmaeilVahedi"));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(

                    //issuer: "https://www.tabrizcraft.ir",
                    //audience: "https://www.tabrizcraft.ir",
                    expires: DateTime.Now.AddHours(3),
                    signingCredentials: credentials,
                    claims: claims
                    );

                new JwtSecurityTokenHandler().WriteToken(token);

                //
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, result.Obj.Token));
                identity.AddClaim(new Claim(ClaimTypes.Name, result.Obj.Fullname));
                identity.AddClaim(new Claim(ClaimTypes.Role, "User"));

                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = true });
                //

                return Success(data: result, message: result.ResultMessage);

            });
        }

        public IActionResult Register()
        {

            return View("LoginContainer", new LoginContainerModel
            {
                RegisterModel = new RegisterModel
                {
                },
                DefaultTab = "register"
            });
        }
        #endregion

        #region Utilities

        private async Task<bool> IsReCaptchaPassedAsync(string gRecaptchaResponse, string secret, HttpRequest request)
        {
            /*if (_appConfig.Value.BypassRecaptcha)
            {
                return true;
            }*/

            using HttpClient httpClient = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("secret", secret),
                new KeyValuePair<string, string>("response", gRecaptchaResponse)
            });
            var res = await httpClient.PostAsync($"https://www.google.com/recaptcha/api/siteverify", content);
            if (res.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }

            string JSONres = res.Content.ReadAsStringAsync().Result;
            dynamic JSONdata = JObject.Parse(JSONres);
            if (JSONdata.success != "true")
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
