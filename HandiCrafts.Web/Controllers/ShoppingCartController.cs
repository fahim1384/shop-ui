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

        public IActionResult Cart()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
        public Task<ResponseState<VoidResult>> InsertCustomerAddress_UI(AddressModel model)
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
                    return Error<VoidResult>(null, message: result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);

            });

        }

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

        public IActionResult InsertAddress()
        {
            HttpClient httpClient = new HttpClient();

            //httpClient.DefaultRequestHeaders.Authorization = _httpClientFactory.CreateClient("myHttpClient").DefaultRequestHeaders.Authorization;

            string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

            GetProvinceListClient client = new GetProvinceListClient(BaseUrl, httpClient);

            List<SelectListItem> Provinces = new List<SelectListItem>();

            var result = client.UIAsync(1).Result;

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

                result.ObjList = new List<OfferDto>() { new OfferDto { Value = 17, MaximumPrice = 1000 } };

               // if (result.ResultCode != 200)
               //     return Error<OfferDtoListResult>(null, message: result.ResultMessage);

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


        //[HttpPost]
        //public Task<ResponseState<>> InsertAddress(CustomerAddress model)
        //{
        //    return TryCatch(async () =>
        //    {

        //        HttpClient httpClient = new HttpClient();

        //        httpClient.DefaultRequestHeaders.Authorization = _httpClientFactory.CreateClient("myHttpClient").DefaultRequestHeaders.Authorization;

        //        string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

        //        InsertCustomerAddressClient client = new InsertCustomerAddressClient(BaseUrl, httpClient);

        //        var result = await client.UIAsync(model);

        //        return Success(data: result, message: result.ResultMessage);

        //    });
        //}

        #endregion

    }
}
