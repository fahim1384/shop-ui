using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HandiCrafts.Web.Models;
using System.Net.Http;
using HandiCrafts.Web.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using System.Threading;
using SmartBreadcrumbs.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HandiCrafts.Web.Controllers
{
    [DefaultBreadcrumb("سرای")]
    public class HomeController : BasePublicController
    {
        private readonly IHttpClientFactory _httpClientFactory;
        //private readonly GetCatProductListClient _client;

        public HomeController(ILogger<HomeController> logger, IStringLocalizer<SharedResource> localizer, IMapper mapper, IHttpClientFactory httpClientFactory/*, GetCatProductListClient client*/) :
        base(logger, localizer, mapper)
        {
            _httpClientFactory = httpClientFactory;
            //_client = client;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public Task<ResponseState<CatProductListResult>> GetCatProductList_UI()
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                GetCatProductListClient client = new GetCatProductListClient(BaseUrl, httpClient);

                var result = await client.UIAsync();

                if (result.ResultCode != 200)
                    return Error<CatProductListResult>(null, message: result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);

            });
        }

        /// <summary>
        /// اسلایدرهای صفحه اصلی
        /// </summary>
        /// <param name="sliderPlaceCode"></param>
        /// <returns></returns>
        [HttpPost]
        public Task<ResponseState<SliderDtoListResult>> GetSliderByPlaceCode_UI(long sliderPlaceCode)
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                GetSliderByPlaceCodeClient client = new GetSliderByPlaceCodeClient(BaseUrl, httpClient);

                var result = await client.UIAsync(sliderPlaceCode);

                if (result.ResultCode != 200)
                    return Error<SliderDtoListResult>(null, message: result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);

            });
        }

        /// <summary>
        /// 5 دسته برتر
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Task<ResponseState<ICollection<CatProduct>>> GetTopCatProductList_UI()
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                GetTopCatProductListClient client = new GetTopCatProductListClient(BaseUrl, httpClient);

                var result = await client.UIAsync();

                if (result.ResultCode != 200)
                    return Error<ICollection<CatProduct>>(null, message: result.ResultMessage);

                return Success(data: result.ObjList, message: result.ResultMessage);

            });
        }

        /// <summary>
        /// لیست نظرات فاخر
        /// </summary>
        /// <returns></returns>
        /// [HttpPost]
        [HttpPost]
        public Task<ResponseState<FamousCommentsDtoListResult>> GetFamousCommentsList_UI()
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                GetFamousCommentsListClient client = new GetFamousCommentsListClient(BaseUrl, httpClient);

                var result = await client.UIAsync();

                if (result.ResultCode != 200)
                    return Error<FamousCommentsDtoListResult>(null, message: result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);
            });
        }

        /// <summary>
        /// لیست کشورها
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Task<ResponseState<LocationListResult>> GetCountryList_UI()
        {
            return TryCatch(async () =>
            {

                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                GetCountryListClient client = new GetCountryListClient(BaseUrl, httpClient);

                var result = await client.UIAsync();

                if (result.ResultCode != 200)
                    return Error<LocationListResult>(null, message: result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);

            });

        }

        /// <summary>
        /// لیست استان ها
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Task<ResponseState<LocationListResult>> GetProvinceList_UI(int countryId)
        {
            return TryCatch(async () =>
            {

                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                GetProvinceListClient client = new GetProvinceListClient(BaseUrl, httpClient);

                var result = await client.UIAsync(countryId);

                if (result.ResultCode != 200)
                    return Error<LocationListResult>(null, message: result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);

            });
        }

        /// <summary>
        /// لیست شهر ها
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Task<ResponseState<LocationListResult>> GetCityList_UI(int provinceId)
        {
            return TryCatch(async () =>
            {

                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                GetCityListClient client = new GetCityListClient(BaseUrl, httpClient);

                var result = await client.UIAsync(provinceId);

                if (result.ResultCode != 200)
                    return Error<LocationListResult>(null, message: result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);

            });
        }

        /// <summary>
        /// لیست انواع بسته بندی محصول
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Task<DefaultResponseState> GetPackingTypeById_UI(long packingtypeId)
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                GetPackingTypeByIdClient client = new GetPackingTypeByIdClient(BaseUrl, httpClient);

                await client.UIAsync(packingtypeId);

                return Success(message: "عملیات با موفقیت انجام شد");
            });
        }

        /// <summary>
        /// لیست انواع بسته بندی
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Task<ResponseState<PackingTypeDtoListResult>> GetPackingTypeList_UI()
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                GetPackingTypeListClient client = new GetPackingTypeListClient(BaseUrl, httpClient);

                var result = await client.UIAsync();

                if (result.ResultCode != 200)
                    return Error<PackingTypeDtoListResult>(null, message: result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);
            });
        }

        /// <summary>
        /// مهر اصالت
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Task<ResponseState<ProductDtoListResult>> GetProductList_HaveMelliCode_UI()
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                HaveMelliCodeClient client = new HaveMelliCodeClient(BaseUrl, httpClient);

                var result = await client.UIAsync();

                if (result.ResultCode != 200)
                    return Error<ProductDtoListResult>(null, message: result.ResultMessage);

                return Success(data:result, message: result.ResultMessage);
            });
        }

        /// <summary>
        /// جدیدترین محصولات
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Task<ResponseState<ProductDtoListResult>> GetProductList_latest_UI()
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                LatestClient client = new LatestClient(BaseUrl, httpClient);

                var result = await client.UIAsync();

                if (result.ResultCode != 200)
                    return Error<ProductDtoListResult>(null, message: result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);

                //return Json(result);
            });
        }

        /// <summary>
        /// بازدیدهای اخیر
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Task<ResponseState<ProductDtoListResult>> GetProductList_LastSeen_UI()
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                LastSeenClient client = new LastSeenClient(BaseUrl, httpClient);

                var result = await client.UIAsync();

                if (result.ResultCode != 200)
                    return Error<ProductDtoListResult>(null, message: result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);
            });
        }
    }
}
