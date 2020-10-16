using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HandiCrafts.Web.Models;
using System.Net.Http;
using HandiCrafts.Web.Infrastructure.Security;
using HandiCrafts.Web.Infrastructure.Framework;
using Microsoft.AspNetCore.Mvc.Localization;
using HandiCrafts.Web.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using System.Threading;

namespace HandiCrafts.Web.Controllers
{
    public class HomeController :Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(/*ILogger<HomeController> logger, IStringLocalizer<SharedResource> localizer, IMapper mapper, */IHttpClientFactory httpClientFactory) //:
            /*base(logger, localizer, mapper)*/
        {
            //_logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            HttpClient httpClient = new HttpClient();

            string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

            //httpClient.DefaultRequestHeaders.Authorization = _httpClientFactory.CreateClient("myHttpClient").DefaultRequestHeaders.Authorization;

            HandiCrafts.Web.BpService.Client client = new BpService.Client(BaseUrl, httpClient);
            
            //using (var httpClient2 = new HttpClient())
            //{
            //    //var contactsClient = new ContactsClient(httpClient2);
            //    //var contacts = await contactsClient.GetContactsAsync();
            //    client.GetProductById(10);
            //}
            //return Success(message: "داده ها ارسال شد");

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

        /*public Task<DefaultResponseState> GetSliderByPlaceCode(long slideId = 1)
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                //httpClient.DefaultRequestHeaders.Authorization = _httpClientFactory.CreateClient("myHttpClient").DefaultRequestHeaders.Authorization;

                HandiCrafts.Web.Client client = new Client(BaseUrl);

                await client.GetSliderByPlaceCodeAsync(slideId, CancellationToken.None);

                return Success(message: "داده ها ارسال شد");
            });
        }

        public Task<DefaultResponseState> GetProductById(long productId = 10)
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                //httpClient.DefaultRequestHeaders.Authorization = _httpClientFactory.CreateClient("myHttpClient").DefaultRequestHeaders.Authorization;

                HandiCrafts.Web.Client client = new Client(BaseUrl);

                var d = client.GetProductByIdAsync(productId);
                d.GetAwaiter().GetResult();

                return Success(message: "داده ها ارسال شد");
            });
        }*/
    }
}
