using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using HandiCrafts.Core.Enums;
using HandiCrafts.Web.Infrastructure.Framework;
using HandiCrafts.Web.Interfaces;
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

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> OnlinePaymentResult(string Authority, string Status)
        {
            CsBankResult bankResult = new CsBankResult()
            {
                Authority = Authority,
                Status = Status
            };

            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = _httpClientFactory.CreateClient("myHttpClient").DefaultRequestHeaders.Authorization;

            string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

            VerifyPaymentClient client = new VerifyPaymentClient(BaseUrl, httpClient);

            var result = await client.UIAsync(Authority, Status);

            if (result.ResultCode != 200)
                return View(new CsBankResult() { Status="NOK", Message= result.Obj });

            bankResult.Message = result.Obj;

            return View(bankResult);

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
