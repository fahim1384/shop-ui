﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using HandiCrafts.Web.Areas.Seller.Models.Account;
using HandiCrafts.Web.Controllers;
using HandiCrafts.Web.Interfaces;
using HandiCrafts.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace HandiCrafts.Web.Areas.Seller.Controllers
{
    [Area("seller")]
    public class AccountController : BasePublicController
    {
        #region Fields

        IHttpClientFactory _httpClientFactory;
        const string _fullname = "fullname";
        const string _seller_userid = "selleruserid";
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

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<ResponseState<LoginRegisterDto>> Login(ShortLoginModel model)
        {
            return TryCatch(async () =>
            {

                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                Client client = new Client(BaseUrl, httpClient);

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

                var result = await client.SellerLoginRegisterAsync(mobile);

                if (result.ResultCode != 200)
                    return Error<LoginRegisterDto>(data: null, message: result.ResultMessage);

                HttpContext.Session.SetString(_seller_userid, result.Obj.UserId.ToString());

                return Success(data: result.Obj, message: result.ResultMessage);

            });

        }

        public IActionResult VerifyCode(long? userid)
        {
            if (userid == null)
            {
                return View("Login");
            }

            return View(new AcceptCodeModel()
            {
                UserId = userid.Value
            });
        }

        [HttpPost]
        public Task<ResponseState<LoginResultDtoSingleResult>> VerifyCode(AcceptCodeModel model)
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = "https://service.tabrizhandicrafts.com/";// _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                ByActivationCodeClient client = new ByActivationCodeClient(BaseUrl, httpClient);

                var result = await client.SellerLogin_ByActivationCode_UI(model.UserId, int.Parse(model.AcceptCode));

                if (result.ResultCode != 200)
                    return Error<LoginResultDtoSingleResult>(null, result.ResultMessage);

                /*string fullnameFromResult = result.Obj.Fullname == null ? HttpContext.Session.GetString(_fullname).ToString() : result.Obj.Fullname;

                result.Obj.Fullname = fullnameFromResult;

                var claims = new[] {
                        new Claim("sellertoken", result.Obj.Token),
                        new Claim("sellerfullname", fullnameFromResult),
                    };

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MyNameIsEsmaeilVahedi"));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(

                    //issuer: "https://www.tabrizcraft.ir",
                    //audience: "https://www.tabrizcraft.ir",
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials,
                    claims: claims
                    );

                new JwtSecurityTokenHandler().WriteToken(token);

                //
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.Surname, result.Obj.Token));
                identity.AddClaim(new Claim(ClaimTypes.Name, fullnameFromResult));
                identity.AddClaim(new Claim(ClaimTypes.Role, "Seller"));

                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = true });*/
                //

                return Success(data: result, message: result.ResultMessage);

            });
        }

        [HttpPost]
        public Task<ResponseState<VoidResult>> Seller_GetActivationCodeForLogin(long mobile)
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                //string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;
                string BaseUrl = "https://service.tabrizhandicrafts.com/";

                SellerClient client = new SellerClient(BaseUrl, httpClient);

                var result = await client.Seller_GetActivationCodeForLogin(null, mobile, null);

                if (result.ResultCode != 200)
                    return Error<VoidResult>(null, result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);

            });
        }

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

                //string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;
                string BaseUrl = "https://service.tabrizhandicrafts.com/";

                ByPassClient client = new ByPassClient(BaseUrl, httpClient);

                var result = await client.SellerLogin_ByPass_UI(model.UserId, model.Password);

                if (result.ResultCode != 200)
                    return Error<LoginResultDtoSingleResult>(null, result.ResultMessage);

                //string fullnameFromResult = result.Obj.Fullname == null ? HttpContext.Session.GetString(_fullname).ToString() : result.Obj.Fullname;

                //result.Obj.Fullname = fullnameFromResult;

                /*var claims = new[] {
                        new Claim("sellertoken", result.Obj.Token),
                        new Claim("sellerfullname", fullnameFromResult),
                    };

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MyNameIsEsmaeilVahedi"));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(

                    //issuer: "https://www.tabrizcraft.ir",
                    //audience: "https://www.tabrizcraft.ir",
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials,
                    claims: claims
                    );

                new JwtSecurityTokenHandler().WriteToken(token);

                //
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.Surname, result.Obj.Token));
                identity.AddClaim(new Claim(ClaimTypes.Name, fullnameFromResult));
                identity.AddClaim(new Claim(ClaimTypes.Role, "Seller"));

                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = true });*/
                //

                return Success(data: result, message: result.ResultMessage);

            });
        }

        [HttpPost]
        public Task<ResponseState<VoidResult>> ForgetPass(ShortLoginModel model)
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                //string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;
                string BaseUrl = "https://service.tabrizhandicrafts.com/";

                SellerLoginClient client = new SellerLoginClient(BaseUrl, httpClient);

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

                var result = await client.ForgetPassAsync(email, mobile);

                if (result.ResultCode != 200)
                    return Error<VoidResult>(null, result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);

            });
        }

        public IActionResult CompleteInformation(long? userid)
        {
            if (userid == null)
            {
                return View("Login");
            }

            PersonalInformationVModel informationVModel;

            try
            {

                #region Ostanha
                HttpClient httpClient = new HttpClient();

                string BaseUrl = "https://service.tabrizhandicrafts.com/"; //_httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                GetProvinceListClient client = new GetProvinceListClient(BaseUrl, httpClient);

                List<SelectListItem> Provinces = new List<SelectListItem>();

                var result = client.UIAsync(null).Result;

                foreach (var item in result.ObjList)
                {
                    Provinces.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.Name });
                }

                ViewBag.Provinces = Provinces;
                #endregion

                #region Sanatgar Full Info
                GetSellerFullInfoClient client1 = new GetSellerFullInfoClient(BaseUrl, httpClient);

                var fullInfo = client1.TestAsync(userid.Value).Result;

                //httpClient.DefaultRequestHeaders.Authorization = _httpClientFactory.CreateClient("myHttpClient2").DefaultRequestHeaders.Authorization;
                //Client client1 = new Client(BaseUrl, httpClient);
                //var fullInfo = client1.GetSellerFullInfo().Result;

                if (fullInfo.ResultCode != 200) throw new Exception(fullInfo.ResultMessage);

                if (fullInfo.ResultCode == 200 && fullInfo.Obj == null) throw new Exception("دیتایی برای این کاربر وجود ندارد");

                PersianDateTime persianDate = new PersianDateTime(fullInfo.Obj.Bdate.Date);
                var Address = fullInfo.Obj.AddressList.Count > 0 ? fullInfo.Obj.AddressList.FirstOrDefault() : null;

                informationVModel = new PersonalInformationVModel()
                {
                    Address = Address != null? Address.Address:null,
                    BirthDate = persianDate.ToPersianDateString(),
                    City = Address != null ? Address.CityId.ToString() : null,
                    FirstName = fullInfo.Obj.Name,
                    LastName = fullInfo.Obj.Fname,
                    Gender = fullInfo.Obj.Gender != null ? fullInfo.Obj.Gender.Value : 1,
                    Lat = Address != null ? Address.Xgps : null,
                    Lng = Address != null ? Address.Ygps : null,
                    MobileNo = fullInfo.Obj.Mobile != null ? "0" + fullInfo.Obj.Mobile.ToString() : null,
                    MobileNo2 = fullInfo.Obj.SecondMobile != null ? "0" + fullInfo.Obj.SecondMobile.ToString() : null,
                    NationalCode = fullInfo.Obj.MelliCode != null ? fullInfo.Obj.MelliCode.Value.ToString() : null,
                    Phone = fullInfo.Obj.Tel != null ? fullInfo.Obj.Tel.Value.ToString() : null,
                    PostalCode = Address != null ? Address.PostalCode.ToString() : null,
                    Province = Address != null ? Address.ProvinceId.ToString() : null,
                    ShabaCode = fullInfo.Obj.ShabaNo,
                    UserId = fullInfo.Obj.SellerId
                };

                #endregion
            }
            catch (Exception ex)
            {
                return View();
            }

            return View(informationVModel);

        }

        [HttpPost]
        public Task<ResponseState<LongResult>> CompleteInformation(PersonalInformationVModel model)
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = "https://service.tabrizhandicrafts.com/";// _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                Client client = new Client(BaseUrl, httpClient);

                PersianDateTime persianDate = PersianDateTime.Parse(model.BirthDate);
                DateTime miladiDate = persianDate.ToDateTime();

                SellerRegisterDto sellerRegisterDto = new SellerRegisterDto()
                {
                    Address = new SellerAddressDto()
                    {
                        Address = model.Address,
                        CityId = long.Parse(model.City),
                        PostalCode = !string.IsNullOrEmpty(model.PostalCode) ? long.Parse(model.PostalCode) : (long?)null,
                        ProvinceId = long.Parse(model.Province),
                        Tel = long.Parse(model.Phone),
                        Xgps = model.Lat,
                        Ygps = model.Lng
                    },
                    Bdate = miladiDate,
                    MelliCode = long.Parse(model.NationalCode),
                    PassWord = model.Password,
                    Mobile = long.Parse(model.MobileNo),
                    Fname = model.FirstName + " " + model.LastName,
                    Name = model.FirstName + " " + model.LastName,
                    Gender = model.Gender,
                    IdentityNo = model.UserId.ToString(),
                    SecondMobile = !string.IsNullOrEmpty(model.MobileNo2) ? long.Parse(model.MobileNo2) : (long?)null,
                    Tel = long.Parse(model.Phone),
                    ShabaNo = model.ShabaCode
                };

                var result = await client.SellerRegisterAsync(sellerRegisterDto);

                if (result.ResultCode != 200)
                    return Error<LongResult>(null, result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);

            });
        }

        public IActionResult Documents(long? userid)
        {
            if (userid == null)
            {
                return View("Login");
            }

            return View(new PersonalInformationVModel()
            {
                UserId = userid.Value
            });
        }

        [HttpPost]
        public Task<ResponseState<DocumentDtoListResult>> GetDocumentListbyRkey(long? rkey)
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = "https://service.tabrizhandicrafts.com/";// _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                Client client = new Client(BaseUrl, httpClient);

                var result = await client.GetDocumentListbyRkeyAsync(rkey);

                if (result.ResultCode != 200)
                    return Error<DocumentDtoListResult>(null, result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);

            });
        }

        [HttpPost]
        public Task<DefaultResponseState> UploadSellerDocument(IFormFile file, string documentId)
        {
            return TryCatch(async () =>
            {
                //HttpClient httpClient = new HttpClient();

                string BaseUrl = "https://service.tabrizhandicrafts.com";// _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                //Client client = new Client(BaseUrl, httpClient);

                HttpClient client = new HttpClient();

                //client.BaseAddress = _httpClientFactory.CreateClient("myHttpClient").BaseAddress;
                //client.DefaultRequestHeaders.Authorization = _httpClientFactory.CreateClient("myHttpClient").DefaultRequestHeaders.Authorization;

                if (file != null && file.Length > 0)
                {
                    try
                    {
                        #region comment
                        /*byte[] data;
                        using (var br = new BinaryReader(file.OpenReadStream()))
                            data = br.ReadBytes((int)file.OpenReadStream().Length);

                        ByteArrayContent bytes = new ByteArrayContent(data);

                        MultipartFormDataContent multiContent = new MultipartFormDataContent();

                        multiContent.Add(bytes, "file", file.FileName);

                        client.PostAsync("UpdateSlider", multiContent).Result;


                        return StatusCode((int)result.StatusCode); //201 Created the request has been fulfilled, resulting in the creation of a new resource.
                        */
                        #endregion

                        var formData = new MultipartFormDataContent();
                        formData.Add(new StreamContent(file.OpenReadStream()), "file", "file");
                        var request = new HttpRequestMessage(HttpMethod.Post, BaseUrl + "/api/Document/UploadSellerDocument?documentId=" + documentId)
                        {
                            Content = formData
                        };

                        request.Headers.Add("accept", "application/json");

                        var response = await client.SendAsync(request);
                        if (response.IsSuccessStatusCode)
                        {
                            var res = await response.Content.ReadAsStringAsync();

                            return Success(message: "آپلود شد");
                        }
                        else
                        {
                            return Error(message: response.ReasonPhrase);
                        }

                    }
                    catch (Exception)
                    {
                        return Error(message: "خطایی رخ داد");
                    }
                }

                /*
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("http://your.url.com/");
                    MultipartFormDataContent form = new MultipartFormDataContent();
                    HttpContent content = new StringContent("fileToUpload");
                    form.Add(content, "fileToUpload");
                    var stream = await file.OpenStreamForReadAsync();
                    content = new StreamContent(stream);
                    content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "fileToUpload",
                        FileName = file.Name
                    };
                    form.Add(content);
                    var response = await client.PostAsync("upload.php", form);
                    return response.Content.ReadAsStringAsync().Result;
                 */

                //await client.UpdateSliderAsync();

                return Error(message: "فایلی انتخاب نشده است");
            });
        }

        #endregion
    }
}
