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
using HandiCrafts.Web.Models.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace HandiCrafts.Web.Controllers
{
    [Authorize(Roles = UserRoleNames.User)]
    public class OrderController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public OrderController(IHttpClientFactory httpClientFactory, ILogger<OrderController> logger, IStringLocalizer<SharedResource> localizer, IMapper mapper)
            :base(logger, localizer, mapper)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult OnlinePaymentResult()
        {
            return View();
        }

        [HttpPost]
        public Task<ResponseState<CsBankResult>> OnlinePaymentResult(string Authority, string Status)
        {
            return TryCatch(async () =>
            {

                HttpClient httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Authorization = _httpClientFactory.CreateClient("myHttpClient").DefaultRequestHeaders.Authorization;

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                VerifyPaymentClient client = new VerifyPaymentClient(BaseUrl, httpClient);

                var result = await client.UIAsync(Authority, Status);

                CsBankResult bankResult = new CsBankResult()
                {
                    Authority = Authority,
                    Status = Status,
                    Message = result.Obj
                };

                if (result.ResultCode != 200)
                    return Error<CsBankResult>(data: null, message: result.ResultMessage);

                return Success(data: bankResult, message: result.Obj);
            });
        }

        [HttpGet]
        public async Task<IActionResult> OrdersHistory()
        {
            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = _httpClientFactory.CreateClient("myHttpClient").DefaultRequestHeaders.Authorization;

            string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

            GetCustomerOrderListClient client = new GetCustomerOrderListClient(BaseUrl, httpClient);

            var result = await client.UIAsync(null);

            if (result.ResultCode != 200)
                return View(null);

            return View(result.ObjList);
        }

        [HttpGet]
        public async Task<IActionResult> OrderById(long orderId)
        {
            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = _httpClientFactory.CreateClient("myHttpClient").DefaultRequestHeaders.Authorization;

            string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

            ByIdClient client = new ByIdClient(BaseUrl, httpClient);

            var result = await client.UIAsync(orderId);

            if (result.ResultCode != 200)
                return View(null);

            return View(result.Obj);
        }
    }
}
