using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HandiCrafts.Core.Domain.Users;
using HandiCrafts.Web.Infrastructure.Security;
using HandiCrafts.Web.Interfaces;
using HandiCrafts.Web.Models.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Namotion.Reflection;
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
                LoginModel = new LoginModel
                {
                    ReturnUrl = returnUrl,
                    //ReCaptchaKey = _configuration["GoogleReCaptcha:key"]//https://codepen.io/api521/pen/NVqbNY
                },
                RegisterModel = new RegisterModel
                {
                    //ReCaptchaKey = _configuration["GoogleReCaptcha:key"]
                },
                DefaultTab = "login"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<ResponseState<string>> Login(LoginModel model)
        {
            return TryCatch(async () =>
            {
                if (!Url.IsLocalUrl(model.ReturnUrl))
                {
                    model.ReturnUrl = null;
                }

                //if (!await IsReCaptchaPassedAsync(Request.Form["g-recaptcha-response"], _configuration["GoogleReCaptcha:secret"], Request))
                //{
                //    return Error<string>(null, Localizer["msg.checkRecaptcha"].Value);
                //}

                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                //httpClient.DefaultRequestHeaders.Authorization = _httpClientFactory.CreateClient("myHttpClient").DefaultRequestHeaders.Authorization;

                HandiCrafts.Web.AuthService.Client client = new AuthService.Client(BaseUrl, httpClient);

                /*client.Login(new AuthService.UserLoginModel()
                 {
                     Username = model.Username,
                     Password = model.Password
                 });*/
                                

                if (model.Username != "148272579" || model.Password != "123456")
                {
                    return Error<string>(null, "مشخصات وارد شده صحیح نمی باشد");
                }

                var claims = new[] {
                        new Claim("Name", "Bobby"),
                        new Claim(JwtRegisteredClaimNames.Email, "hello@yogihosting.com"),
                };

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MynameisJamesBond007"));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(

                    issuer: "https://www.yogihosting.com",
                    audience: "https://www.yogihosting.com",
                    expires: DateTime.Now.AddHours(3),
                    signingCredentials: credentials,
                    claims: claims
                    );

                new JwtSecurityTokenHandler().WriteToken(token);


                if (!string.IsNullOrEmpty(model.ReturnUrl))
                {
                    return Success<string>(model.ReturnUrl);
                }
                else
                {
                    return Success<string>("/user");
                }

            });

        }

        public IActionResult VerifyCode()
        {

            return View("LoginContainer", new LoginContainerModel
            {
                RegisterModel = new RegisterModel
                {
                },
                DefaultTab = "verifyCode"
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
