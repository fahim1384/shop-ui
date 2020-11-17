using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using HandiCrafts.Core.Enums;
using HandiCrafts.Web.Infrastructure.Framework;
using HandiCrafts.Web.Interfaces;
using HandiCrafts.Web.Models;
using HandiCrafts.Web.Models.ShoppingCart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace HandiCrafts.Web.Controllers
{
    //[Authorize(Roles = UserRoleNames.User)]
    public class ShoppingCartController : BasePublicController
    {
        #region Fields

        IHttpClientFactory _httpClientFactory;
        const string BankUrl = "BankUrl";
        const string CustomerOrderId = "CustomerOrderId";
        const string OrderNo = "OrderNo";
        const string PostPrice = "PostPrice";
        const string RedirectToBank = "RedirectToBank";

        #endregion

        #region Constructors

        public ShoppingCartController(
            IHttpClientFactory httpClientFactory,
            ILogger<AccountController> logger,
            IStringLocalizer<SharedResource> localizer,
            IMapper mapper) : base(logger, localizer, mapper)
        {
            _httpClientFactory = httpClientFactory;
        }


        #endregion

        #region Actions

        [Authorize(Roles = UserRoleNames.User)]
        public IActionResult Cart()
        {
            return View();
        }

        [HttpPost]
        public Task<ResponseState<CustomerAddressDtoSingleResult>> GetCustomerDefultAddress_UI()
        {
            return TryCatch(async () =>
            {

                HttpClient httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Authorization = _httpClientFactory.CreateClient("myHttpClient").DefaultRequestHeaders.Authorization;

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                GetCustomerDefultAddressClient client = new GetCustomerDefultAddressClient(BaseUrl, httpClient);

                var result = await client.UIAsync();

                return Success(data: result, message: result.ResultMessage);

            });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoleNames.User)]
        public Task<ResponseState<CustomerAddressDtoListResult>> GetCustomerAddressList_UI()
        {
            return TryCatch(async () =>
            {

                HttpClient httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Authorization = _httpClientFactory.CreateClient("myHttpClient").DefaultRequestHeaders.Authorization;

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                GetCustomerAddressListClient client = new GetCustomerAddressListClient(BaseUrl, httpClient);

                var result = await client.UIAsync();

                return Success(data: result, message: result.ResultMessage);

            });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoleNames.User)]
        public Task<ResponseState<Int64SingleResult>> InsertCustomerAddress_UI(AddressModel model)
        {
            return TryCatch(async () =>
            {

                HttpClient httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Authorization = _httpClientFactory.CreateClient("myHttpClient").DefaultRequestHeaders.Authorization;

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                InsertCustomerAddressClient client = new InsertCustomerAddressClient(BaseUrl, httpClient);

                var customerAddress = new CustomerAddressDto()
                {
                    Address = model.PostalAddress,
                    CityId = model.CityId,
                    DefualtAddress = model.DefualtAddress,
                    IssureFamily = model.LastName,
                    IssureMelliCode = long.Parse(model.NationalCode),
                    IssureMobile = long.Parse(model.MobileNo),
                    IssureName = model.FirstName,
                    PostalCode = long.Parse(model.PostalCode),
                    ProvinceId = model.SatatId,
                    Tel = model.Tel,
                    Titel = model.Titel
                };

                var result = await client.UIAsync(customerAddress);

                if (result.ResultCode != 200)
                    return Error<Int64SingleResult>(null, message: result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);

            });

        }

        [Authorize(Roles = UserRoleNames.User)]
        public IActionResult Address()
        {
            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = _httpClientFactory.CreateClient("myHttpClient").DefaultRequestHeaders.Authorization;

            string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

            GetCustomerAddressListClient client = new GetCustomerAddressListClient(BaseUrl, httpClient);

            var result = client.UIAsync().Result;

            return View("Address", new AddressContainerModel
            {
                AddressList = result.ObjList
            });
        }

        [Authorize(Roles = UserRoleNames.User)]
        public IActionResult InsertAddress()
        {
            HttpClient httpClient = new HttpClient();

            //httpClient.DefaultRequestHeaders.Authorization = _httpClientFactory.CreateClient("myHttpClient").DefaultRequestHeaders.Authorization;

            string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

            GetProvinceListClient client = new GetProvinceListClient(BaseUrl, httpClient);

            List<SelectListItem> Provinces = new List<SelectListItem>();

            var result = client.UIAsync(null).Result;

            foreach (var item in result.ObjList)
            {
                Provinces.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.Name });
            }
            //Provinces.Add(new SelectListItem { Value = "4", Text = "آذربایجان" });

            ViewBag.Provinces = Provinces;

            return View();
        }

        [HttpPost]
        public Task<ResponseState<LocationDtoListResult>> GetCityList_UI(long provinceId)
        {
            return TryCatch(async () =>
            {

                HttpClient httpClient = new HttpClient();

                //httpClient.DefaultRequestHeaders.Authorization = _httpClientFactory.CreateClient("myHttpClient").DefaultRequestHeaders.Authorization;

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                GetCityListClient client = new GetCityListClient(BaseUrl, httpClient);

                var result = await client.UIAsync(provinceId);

                return Success(data: result, message: result.ResultMessage);

            });

        }

        /// <summary>
        /// مشخصات محصول براساس لیست آیدی محصول
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public Task<ResponseState<ProductDtoListResult>> GetProductByIdList_UI(List<long> model)
        {
            return TryCatch(async () =>
            {

                HttpClient httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Authorization = _httpClientFactory.CreateClient("myHttpClient").DefaultRequestHeaders.Authorization;

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                GetProductByIdListClient client = new GetProductByIdListClient(BaseUrl, httpClient);

                var result = await client.UIAsync(model);

                if (result.ResultCode != 200)
                    return Error<ProductDtoListResult>(null, message: result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);

            });

        }

        /// <summary>لیست پیکج های فعال به همراه عکس ها</summary> ///
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public Task<ResponseState<OfferDtoListResult>> GetOfferValueByCode_UI(string offerCode)
        {
            return TryCatch(async () =>
            {

                HttpClient httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Authorization = _httpClientFactory.CreateClient("myHttpClient").DefaultRequestHeaders.Authorization;

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                GetOfferValueByCodeClient client = new GetOfferValueByCodeClient(BaseUrl, httpClient);

                var result = await client.UIAsync(offerCode);

                //result.ObjList = new List<OfferDto>() { new OfferDto { Value = 17, MaximumPrice = 1000 } };

                if (result.ResultCode != 200)
                    return Error<OfferDtoListResult>(null, message: result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);

            });

        }

        /// <summary>
        /// لیست شیوه های پرداخت
        /// </summary>
        /// <returns></returns>
        public Task<ResponseState<PaymentTypeDtoListResult>> GetPaymentTypeList_UI()
        {
            return TryCatch(async () =>
            {

                HttpClient httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Authorization = _httpClientFactory.CreateClient("myHttpClient").DefaultRequestHeaders.Authorization;

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                GetPaymentTypeListClient client = new GetPaymentTypeListClient(BaseUrl, httpClient);

                var result = await client.UIAsync();

                if (result.ResultCode != 200)
                    return Error<PaymentTypeDtoListResult>(null, message: result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);

            });
        }

        /// <summary>
        /// دریافت قیمت و اطلاعات سبد خرید
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = UserRoleNames.User)]
        public Task<ResponseState<OrderPreViewResultDtoSingleResult>> CustomerOrderPreview(OrderModel model)
        {
            return TryCatch(async () =>
            {

                HttpClient httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Authorization = _httpClientFactory.CreateClient("myHttpClient").DefaultRequestHeaders.Authorization;

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                CustomerOrderPreviewClient client = new CustomerOrderPreviewClient(BaseUrl, httpClient);

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(model, Newtonsoft.Json.Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings
                {
                    DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Populate
                });

                //***************************************************
                httpClient.BaseAddress = _httpClientFactory.CreateClient("myHttpClient").BaseAddress;
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
                // Method  
                HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/Product/CustomerOrderPreview_UI", model);

                var result = await response.Content.ReadAsAsync<OrderPreViewResultDtoSingleResult>();

                //*********************************************************

                //var result2 = await client.UIAsync(model);

                if (result.ResultCode != 200)
                    return Error<OrderPreViewResultDtoSingleResult>(null, message: result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);

            });
        }

        /// <summary>
        /// ثبت سفارش
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = UserRoleNames.User)]
        public Task<ResponseState<InsertOrderResultDtoSingleResult>> InsertCustomerOrder_UI(OrderModel model)
        {
            return TryCatch(async () =>
            {

                HttpClient httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Authorization = _httpClientFactory.CreateClient("myHttpClient").DefaultRequestHeaders.Authorization;

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                InsertCustomerOrderClient client = new InsertCustomerOrderClient(BaseUrl, httpClient);

                var result = await client.UIAsync(model);

                if (result.ResultCode != 200)
                    return Error<InsertOrderResultDtoSingleResult>(null, message: result.ResultMessage);

                HttpContext.Session.SetString(BankUrl, result.Obj.BankUrl);
                HttpContext.Session.SetString(CustomerOrderId, result.Obj.CustomerOrderId != null ? result.Obj.CustomerOrderId.ToString() : null);
                HttpContext.Session.SetString(OrderNo, result.Obj.OrderNo != null ? result.Obj.OrderNo.ToString() : null);
                HttpContext.Session.SetString(PostPrice, result.Obj.PostPrice != null ? result.Obj.PostPrice.ToString() : null);
                HttpContext.Session.SetString(RedirectToBank, result.Obj.RedirectToBank.ToString());

                return Success(data: result, message: result.ResultMessage);

            });
        }
        #endregion

    }
}
