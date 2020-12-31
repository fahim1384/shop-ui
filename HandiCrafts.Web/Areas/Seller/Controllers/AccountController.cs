using System;
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
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace HandiCrafts.Web.Areas.Seller.Controllers
{
    [Area("seller")]
    public class AccountController : BasePublicController
    {
        #region Fields

        IHttpClientFactory _httpClientFactory;
        const string _fullname = "fullname";
        const string _seller_userid = "selleruserid";
        public IConfiguration _configuration;
        #endregion

        #region Constructors

        public AccountController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<AccountController> logger,
            IStringLocalizer<SharedResource> localizer,
            IMapper mapper) : base(logger, localizer, mapper)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
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

                string BaseUrl = _configuration["BaseUrl"];

                ByActivationCodeClient client = new ByActivationCodeClient(BaseUrl, httpClient);

                var result = await client.SellerLogin_ByActivationCode_UI(model.UserId, int.Parse(model.AcceptCode));

                if (result.ResultCode != 200)
                    return Error<LoginResultDtoSingleResult>(null, result.ResultMessage);

                Response.Cookies.Append(
                    "sellertoken",
                    result.Obj.Token,
                    new CookieOptions()
                    {
                        Expires = DateTime.Now.AddHours(1),
                        IsEssential = true
                    });

                return Success(data: result, message: result.ResultMessage);

            });
        }

        [HttpPost]
        public Task<ResponseState<VoidResult>> Seller_GetActivationCodeForLogin(long mobile)
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = _configuration["BaseUrl"];

                SellerClient client = new SellerClient(BaseUrl, httpClient);

                var result = await client.GetActivationCodeForLoginAsync(null, mobile, null);

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

                string BaseUrl = _configuration["BaseUrl"];

                ByPassClient client = new ByPassClient(BaseUrl, httpClient);

                var result = await client.SellerLogin_ByPass_UI(model.UserId, model.Password);

                if (result.ResultCode != 200)
                    return Error<LoginResultDtoSingleResult>(null, result.ResultMessage);

                Response.Cookies.Append(
                    "sellertoken",
                    result.Obj.Token,
                    new CookieOptions()
                    {
                        Expires = DateTime.Now.AddHours(1),
                        IsEssential = true
                    });

                return Success(data: result, message: result.ResultMessage);

            });
        }

        [HttpPost]
        public Task<ResponseState<VoidResult>> ForgetPass(ShortLoginModel model)
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = _configuration["BaseUrl"];

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

        public async Task<IActionResult> CompleteInformation()
        {
            if (string.IsNullOrEmpty(Request.Cookies["sellertoken"]))
            {
                return View("Login");
            }

            PersonalInformationVModel informationVModel;

            try
            {

                #region Ostanha
                HttpClient httpClient = new HttpClient();

                string BaseUrl = _configuration["BaseUrl"];

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

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["sellertoken"]);
                Client client2 = new Client(BaseUrl, httpClient);
                var fullInfo = await client2.GetSellerFullInfoAsync();

                if (fullInfo.ResultCode != 200) throw new Exception(fullInfo.ResultMessage);

                if (fullInfo.ResultCode == 200 && fullInfo.Obj == null) throw new Exception("دیتایی برای این کاربر وجود ندارد");

                PersianCalendar pc = new PersianCalendar();

                string persianDate = string.Format("{0}/{1}/{2}", pc.GetYear(fullInfo.Obj.Bdate.DateTime).ToString().PadLeft(4, '0'),
                    pc.GetMonth(fullInfo.Obj.Bdate.DateTime).ToString().PadLeft(2, '0'),
                    pc.GetDayOfMonth(fullInfo.Obj.Bdate.DateTime).ToString().PadLeft(2, '0'));


                //string persianDate = HandiCrafts.Core.Infrastructure.DateTimeExtentions.ToPersianDate(fullInfo.Obj.Bdate.DateTime);

                var Address = fullInfo.Obj.AddressList.Count > 0 ? fullInfo.Obj.AddressList.FirstOrDefault() : null;

                string melicode = null;
                if (fullInfo.Obj.MelliCode != null)
                {
                    melicode = fullInfo.Obj.MelliCode.Value.ToString();

                    if (fullInfo.Obj.MelliCode.Value.ToString().Length != 10)
                    {
                        var l = 10 - fullInfo.Obj.MelliCode.Value.ToString().Length;
                        for (int i = 0; i < l; i++)
                        {
                            melicode = "0" + melicode;
                        }
                    }
                }

                informationVModel = new PersonalInformationVModel()
                {
                    Address = Address != null ? Address.Address : null,
                    BirthDate = persianDate,
                    City = Address != null ? Address.CityId.ToString() : null,
                    FirstName = fullInfo.Obj.Fname,
                    LastName = fullInfo.Obj.Name,
                    Gender = fullInfo.Obj.Gender != null ? fullInfo.Obj.Gender.Value : 1,
                    Lat = Address != null ? Address.Xgps : null,
                    Lng = Address != null ? Address.Ygps : null,
                    MobileNo = fullInfo.Obj.Mobile != null ? "0" + fullInfo.Obj.Mobile.ToString() : null,
                    MobileNo2 = fullInfo.Obj.SecondMobile != null ? "0" + fullInfo.Obj.SecondMobile.ToString() : null,
                    NationalCode = melicode,
                    Phone = fullInfo.Obj.Tel != null ? "0" + fullInfo.Obj.Tel.Value.ToString() : null,
                    PostalCode = Address != null ? Address.PostalCode.ToString() : null,
                    Province = Address != null ? Address.ProvinceId.ToString() : null,
                    ShabaCode = fullInfo.Obj.ShabaNo,
                    UserId = fullInfo.Obj.SellerId,
                    AddOrEditModel = 2,
                    ShenasnameNo = fullInfo.Obj.IdentityNo,
                    AddressId = Address != null ? Address.Id : (long?)null,
                };

                #endregion
            }
            catch (UnauthorizedAccessException)
            {
                ModelState.AddModelError("401", "زمان توکن شما به اتمام رسیده است.لطفا از اول عملیات را انجام دهید");
                return View();
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
                if (string.IsNullOrEmpty(Request.Cookies["sellertoken"]))
                    return Error<LongResult>(null, message: "مهلت زمانی شما برای ادامه عملیات به پایان رسیده است لطفا مراحل ورود را از اول شروع کنید");

                HttpClient httpClient = new HttpClient();

                string BaseUrl = _configuration["BaseUrl"];

                Client client = new Client(BaseUrl, httpClient);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["sellertoken"]);

                //DateTime miladiDate2 = Core.Infrastructure.DateTimeExtentions.ToGregorianDateTime(model.BirthDate).Value;

                string dat, sal, mah, roz, ret;
                dat = model.BirthDate;
                sal = dat.Substring(0, 4);
                mah = dat.Substring(5, 2);
                roz = dat.Substring(8, 2);
                PersianCalendar pc = new PersianCalendar();
                ret = pc.ToDateTime(Convert.ToInt32(sal), Convert.ToInt32(mah), Convert.ToInt32(roz), 0, 0, 0, 0).ToString();

                DateTime miladiDate2 = DateTime.Parse(ret.ToString());

                long AddressId = 0;
                if (model.AddressId != null) AddressId = model.AddressId.Value;

                SellerRegisterDto sellerRegisterDto = new SellerRegisterDto()
                {
                    Address = new SellerAddressDto()
                    {
                        Id = AddressId,
                        Address = model.Address,
                        CityId = long.Parse(model.City),
                        PostalCode = !string.IsNullOrEmpty(model.PostalCode) ? long.Parse(model.PostalCode) : (long?)null,
                        ProvinceId = long.Parse(model.Province),
                        Tel = long.Parse(model.Phone),
                        Xgps = model.Lat,
                        Ygps = model.Lng
                    },
                    Bdate = miladiDate2,
                    MelliCode = long.Parse(model.NationalCode),
                    PassWord = model.Password,
                    Mobile = long.Parse(model.MobileNo),
                    Fname = model.FirstName,
                    Name = model.LastName,
                    Gender = model.Gender,
                    IdentityNo = model.ShenasnameNo, //model.UserId.ToString(),
                    SecondMobile = !string.IsNullOrEmpty(model.MobileNo2) ? long.Parse(model.MobileNo2) : (long?)null,
                    Tel = long.Parse(model.Phone),
                    ShabaNo = model.ShabaCode
                };

                var result = await client.UpdateSellerFullInfoAsync(sellerRegisterDto);

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
                if (string.IsNullOrEmpty(Request.Cookies["sellertoken"]))
                    return Error<DocumentDtoListResult>(null, message: "مهلت زمانی شما برای ادامه عملیات به پایان رسیده است لطفا مراحل ورود را از اول شروع کنید");

                HttpClient httpClient = new HttpClient();

                string BaseUrl = _configuration["BaseUrl"];

                Client client = new Client(BaseUrl, httpClient);

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["sellertoken"]);

                var result = await client.GetDocumentListbyRkeyAsync(rkey);

                if (result.ResultCode != 200)
                    return Error<DocumentDtoListResult>(null, result.ResultMessage);

                #region Get Uploded Files

                #endregion
                return Success(data: result, message: result.ResultMessage);

            });
        }

        [HttpPost]
        public Task<DefaultResponseState> UploadSellerDocument(IFormFile upload, string documentId)
        {
            return TryCatch(async () =>
            {

                string BaseUrl = _configuration["BaseUrl"];

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["sellertoken"]);
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"/*upload.ContentType*/));//ACCEPT header

                if (upload != null && upload.Length > 0)
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
                        formData.Add(new StreamContent(upload.OpenReadStream()), "Document", upload.FileName);
                        var request = new HttpRequestMessage(HttpMethod.Post, BaseUrl + "api/Document/UploadSellerDocument?documentId=" + documentId)
                        {
                            Content = formData
                        };

                        //request.Content.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");

                        //request.Headers.Add("accept", "application/json");
                        //request.Headers.Add("Content-Type", upload.ContentType);
                        request.Headers.Add("Authorization", "Bearer " + Request.Cookies["sellertoken"]);

                        var response = await client.SendAsync(request);
                        if (response.IsSuccessStatusCode)
                        {
                            var res = await (response.Content.ReadAsStringAsync());

                            var fileUploadResult = Newtonsoft.Json.JsonConvert.DeserializeObject<FileUpload>(res);

                            if (fileUploadResult.resultCode != 200)
                                return Error(message: fileUploadResult.resultMessage);

                            return Success(message: "آپلود شد");
                        }
                        else
                        {
                            return Error(message: response.ReasonPhrase);
                        }

                    }
                    catch (Exception ex)
                    {
                        return Error(message: "خطایی رخ داد");
                    }
                }

                return Error(message: "فایلی انتخاب نشده است");
            });
        }

        public class FileUpload
        {
            public int? resultCode { get; set; }
            public string resultMessage { get; set; }
        }

        public IActionResult welcome()
        {
            HttpClient httpClient = new HttpClient();

            string BaseUrl = _configuration["BaseUrl"];

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["sellertoken"]);

            SellerRegisterConfirmClient client = new SellerRegisterConfirmClient(BaseUrl, httpClient);

            var result = client.UIAsync().Result;

            if (result.ResultCode != 200)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public Task<ResponseState<VoidResult>> ForgetPassword(ShortLoginModel model)
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = _configuration["BaseUrl"];

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

        public IActionResult ChangePassword(string emailmobile)
        {
            return View("ChangePassword", new SetNewPassword()
            {
                EmailorMobileNo = emailmobile
            });
        }

        [HttpPost]
        public Task<ResponseState<VoidResult>> ChangePassword(SetNewPassword model)
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = _configuration["BaseUrl"];

                SellerClient client = new SellerClient(BaseUrl, httpClient);

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

                var result = await client.ChangePassByActivationCodeAsync(email, mobile, int.Parse(model.AcceptCode), model.Password);

                if (result.ResultCode != 200)
                    return Error<VoidResult>(null, result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);

            });
        }

        [HttpPost]
        public Task<ResponseState<List<SellerDocumentDto>>> GetUploadedDocuments()
        {
            return TryCatch(async () =>
            {

                HttpClient httpClient = new HttpClient();

                string BaseUrl = _configuration["BaseUrl"];

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["sellertoken"]);
                Client client2 = new Client(BaseUrl, httpClient);
                var fullInfo = await client2.GetSellerFullInfoAsync();

                if (fullInfo.ResultCode != 200) return Error<List<SellerDocumentDto>>(data: null, fullInfo.ResultMessage);

                if (fullInfo.ResultCode == 200 && fullInfo.Obj == null) return Error<List<SellerDocumentDto>>(data: null, "دیتایی برای این کاربر وجود ندارد");

                List<SellerDocumentDto> sellerDocumentDtos = new List<SellerDocumentDto>();

                sellerDocumentDtos = fullInfo.Obj.DocumentList.ToList();

                return Success<List<SellerDocumentDto>>(data: sellerDocumentDtos, message: fullInfo.ResultMessage);
            });
        }

        [HttpPost]
        public Task<ResponseState<PersonalInformationVModel>> GetSellerFullInfoForContract()
        {
            return TryCatch(async () =>
            {
                if (string.IsNullOrEmpty(Request.Cookies["sellertoken"]))
                {
                    return Error<PersonalInformationVModel>(data: null, message: "مهلت زمانی استفاده از توکن به پایان رسید لطفا مراحل ثبت نام را از ابتدا اامه دهید");
                }

                HttpClient httpClient = new HttpClient();

                string BaseUrl = _configuration["BaseUrl"];

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["sellertoken"]);
                Client client2 = new Client(BaseUrl, httpClient);
                var fullInfo = await client2.GetSellerFullInfoAsync();

                if (fullInfo.ResultCode != 200)
                    return Error<PersonalInformationVModel>(data: null, message: fullInfo.ResultMessage);

                if (fullInfo.ResultCode == 200 && fullInfo.Obj == null)
                    return Error<PersonalInformationVModel>(data: null, message: "دیتایی برای این کاربر وجود ندارد");

                string persianDate = Core.Infrastructure.DateTimeExtentions.ToPersianDate(fullInfo.Obj.Bdate.Date);

                var Address = fullInfo.Obj.AddressList.Count > 0 ? fullInfo.Obj.AddressList.FirstOrDefault() : null;

                PersonalInformationVModel informationVModel = new PersonalInformationVModel()
                {
                    Address = Address != null ? Address.Address : null,
                    BirthDate = persianDate,
                    City = Address != null ? Address.CityId.ToString() : null,
                    FirstName = fullInfo.Obj.Name,
                    LastName = fullInfo.Obj.Fname,
                    Gender = fullInfo.Obj.Gender != null ? fullInfo.Obj.Gender.Value : 1,
                    Lat = Address != null ? Address.Xgps : null,
                    Lng = Address != null ? Address.Ygps : null,
                    MobileNo = fullInfo.Obj.Mobile != null ? "0" + fullInfo.Obj.Mobile.ToString() : null,
                    MobileNo2 = fullInfo.Obj.SecondMobile != null ? "0" + fullInfo.Obj.SecondMobile.ToString() : null,
                    NationalCode = fullInfo.Obj.MelliCode != null ? fullInfo.Obj.MelliCode.Value.ToString() : null,
                    Phone = fullInfo.Obj.Tel != null ? "0" + fullInfo.Obj.Tel.Value.ToString() : null,
                    PostalCode = Address != null ? Address.PostalCode.ToString() : null,
                    Province = Address != null ? Address.ProvinceId.ToString() : null,
                    ShabaCode = fullInfo.Obj.ShabaNo,
                    UserId = fullInfo.Obj.SellerId,
                    AddOrEditModel = 2,
                    ShenasnameNo = fullInfo.Obj.IdentityNo
                };

                return Success<PersonalInformationVModel>(data: informationVModel, message: fullInfo.ResultMessage);

            });
        }

        [HttpPost]
        public Task<ResponseState<VoidResult>> SellerRegisterConfirm_UI()
        {
            return TryCatch(async () =>
            {
                if (string.IsNullOrEmpty(Request.Cookies["sellertoken"]))
                {
                    return Error<VoidResult>(data: null, message: "مهلت زمانی استفاده از توکن به پایان رسید لطفا مراحل ثبت نام را از ابتدا اامه دهید");
                }

                HttpClient httpClient = new HttpClient();

                string BaseUrl = _configuration["BaseUrl"];

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["sellertoken"]);

                SellerRegisterConfirmClient client2 = new SellerRegisterConfirmClient(BaseUrl, httpClient);

                var result = await client2.UIAsync();

                if (result.ResultCode != 200)
                    return Error<VoidResult>(data: null, message: result.ResultMessage);

                return Success<VoidResult>(data: null, message: result.ResultMessage);

            });
        }
    }

    #endregion
}