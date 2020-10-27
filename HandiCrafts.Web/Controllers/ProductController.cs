using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartBreadcrumbs.Attributes;
using SmartBreadcrumbs.Nodes;

namespace HandiCrafts.Web.Controllers
{
    public class ProductController : Controller
    {
        //[DefaultBreadcrumb("خانه")]
        [Breadcrumb("ViewData.BreadcrumbNode")]
        public IActionResult Index()
        {
            var childNode1 = new MvcBreadcrumbNode("Index", "Product", "دسته بندی 1");

            var childNode2 = new MvcControllerBreadcrumbNode("Product", "زیر دسته")
            {
                OverwriteTitleOnExactMatch = true,
                Parent = childNode1
            };

            var childNode3 = new RazorPageBreadcrumbNode("/Path", "زیر زیر دسته")
            {
                OverwriteTitleOnExactMatch = true,
                Parent = childNode2
            };

            ViewData["BreadcrumbNode"] = childNode3;

            return View();
        }

        public IActionResult ByFilter()
        {
            return View();
        }

        /// <summary>
        /// لیست رنگ بندی محصول براس کد محصول
        /// </summary>
        /// <returns> /api/Product/GetProductColorList_UI </returns>
        //public async Task<JsonResult> GetProductColorList_UI(int productId)
        //{
        //    var client = new RestClient("https://service.tabrizhandicrafts.com/api/Product/GetProductColorList_UI?productId=" + productId);
        //    var request = new RestRequest(Method.GET);
        //    var response = new RestResponse();
        //    Task.Run(async () =>
        //    {
        //        response = await GetResponseContentAsync(client, request) as RestResponse;
        //    }).Wait();
        //    var jsonResponse = JsonConvert.DeserializeObject<JObject>(response.Content);


        //    return Json(jsonResponse);
        //}

        /// <summary>
        /// لیست محصولات
        /// </summary>
        /// <returns> /api/Product/GetProductList </returns>
        //public async Task<JsonResult> GetProductList()
        //{
        //    var client = new RestClient("https://service.tabrizhandicrafts.com/api/Product/GetProductList");
        //    var request = new RestRequest(Method.GET);
        //    var response = new RestResponse();
        //    Task.Run(async () =>
        //    {
        //        response = await GetResponseContentAsync(client, request) as RestResponse;
        //    }).Wait();
        //    var jsonResponse = JsonConvert.DeserializeObject<JObject>(response.Content);


        //    return Json(jsonResponse);
        //}

        // =========== Private =============
        //public static Task<IRestResponse> GetResponseContentAsync(RestClient theClient, RestRequest theRequest)
        //{
        //    var tcs = new TaskCompletionSource<IRestResponse>();
        //    theClient.ExecuteAsync(theRequest, response =>
        //    {
        //        tcs.SetResult(response);
        //    });
        //    return tcs.Task;
        //}
    }
}
