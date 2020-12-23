using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HandiCrafts.Web.Interfaces;
using HandiCrafts.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartBreadcrumbs.Attributes;
using SmartBreadcrumbs.Nodes;

namespace HandiCrafts.Web.Controllers
{
    public class ProductController : BasePublicController
    {
        IHttpClientFactory _httpClientFactory;

        public ProductController(ILogger<ProductController> logger, IStringLocalizer<SharedResource> localizer, IMapper mapper, IHttpClientFactory httpClientFactory) :
            base(logger, localizer, mapper)
        {
            _httpClientFactory = httpClientFactory;
        }

        //[DefaultBreadcrumb("خانه")]
        [Breadcrumb("ViewData.BreadcrumbNode")]
        public IActionResult Index(int? id)
        {
            if (id == null)
                return Redirect("/Product/ByFilter");

            HttpClient httpClient = new HttpClient();

            string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

            GetProductByIdClient client = new GetProductByIdClient(BaseUrl, httpClient);

            ProductDtoSingleResult result;
            try
            {
                result = client.UIAsync(id).Result;
            }
            catch (Exception ex)
            {
                return View();
                //throw;
            }

            if (result.ResultCode == 200)
            {
                //    ============== category ===============
                /*GetCatProductListByParentIdClient client2 = new GetCatProductListByParentIdClient(BaseUrl, httpClient);

                var result2 = client2.UIAsync(result.Obj.CatProductId);

                var childNode1 = new MvcBreadcrumbNode("Index", "/Product", result.Obj.CatProductName);

                foreach (var item in result2.Result.ObjList)
                {
                    var childNode2 = new MvcControllerBreadcrumbNode("/Product", item.Name)
                    {
                        OverwriteTitleOnExactMatch = true,
                        Parent = childNode1
                    };
                }*/

                var childNode1 = new MvcBreadcrumbNode("Index", "/Product", result.Obj.CatProductName);
                ViewData["BreadcrumbNode"] = childNode1;

                ProductDtoUI productDtoUI = new ProductDtoUI()
                {
                    AparatUrl = result.Obj.AparatUrl,
                    CatProductId = result.Obj.CatProductId,
                    CatProductName = result.Obj.CatProductName,
                    Coding = result.Obj.Coding,
                    Count = result.Obj.Count,
                    CoverImageHurl = result.Obj.CoverImageHurl,
                    CoverImageUrl = result.Obj.CoverImageUrl,
                    Description = result.Obj.Description,
                    FinalStatus = result.Obj.FinalStatus,
                    FinalStatusId = result.Obj.FinalStatusId,
                    Id = result.Obj.Id,
                    KeyWords = result.Obj.KeyWords,
                    LastSeenDate = result.Obj.LastSeenDate,
                    MelliCode = result.Obj.MelliCode,
                    MelliFlag = result.Obj.MelliFlag,
                    Name = result.Obj.Name,
                    OfferAmount = result.Obj.OfferAmount,
                    OfferId = result.Obj.OfferId,
                    OfferPercent = result.Obj.OfferPercent,
                    Price = result.Obj.Price,
                    PriceAftterOffer = result.Obj.PriceAftterOffer,
                    Rating = result.Obj.Rating,
                    Score = result.Obj.Score,
                    SeenCount = result.Obj.SeenCount,
                    SellerId = result.Obj.SellerId,
                    SellerName = result.Obj.SellerName,
                    UnescoCode = result.Obj.UnescoCode,
                    UnescoFlag = result.Obj.UnescoFlag,
                    Weight = result.Obj.Weight,
                    images=new List<ProductImageDto>()
                };

                GetImageListByProductIdClient client2 = new GetImageListByProductIdClient(BaseUrl, httpClient);

                var imgResult = client2.UIAsync(id).Result;

                if (imgResult.ResultCode == 200)
                {
                    if (imgResult.ObjList.Count > 0)
                    {
                        //productDtoUI.images = new List<ProductImageDto>();

                        ProductImageDto productImageDto;
                        foreach (var item in imgResult.ObjList)
                        {
                            productImageDto = new ProductImageDto()
                            {
                                FileType = item.FileType,
                                Id = item.Id,
                                ImageUrl = item.ImageUrl,
                                Title = item.Title,
                                ProductId = item.ProductId
                            };

                            productDtoUI.images.Add(productImageDto);
                        }
                    }
                }
                return View(productDtoUI);
            }

            /*var childNode1 = new MvcBreadcrumbNode("Index", "Product", "دسته بندی 1");

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

            ViewData["BreadcrumbNode"] = childNode3;*/

            return View();
        }

        public class ProductDtoUI : ProductDto
        {
            public List<ProductImageDto> images { get; set; }
        }

        public IActionResult ByFilter()
        {
            return View();
        }

        /// <summary>
        /// لیست محصولات باصفحه بنذی و فیلتر
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Task<ResponseState<ProductListDtoSingleResult>> GetProductList_Paging_Filtering_UI(long? catProductId = null, string productName = null, long? minPrice = null,
            long? maxPrice = null, int? sortMethod = null, int? pageSize = 12, int? pageNumber = 1, List<long> sellerIdList = null)
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                FilteringClient client = new FilteringClient(BaseUrl, httpClient);

                var result = await client.UIAsync(new ProductListParam()
                {
                    CatProductId = catProductId,
                    ProductName = productName,
                    MaxPrice = maxPrice,
                    MinPrice = minPrice,
                    SellerIdList = sellerIdList,
                    SortMethod = sortMethod,
                    PageNumber = pageNumber.Value,
                    PageSize = pageSize.Value
                });

                if (result.ResultCode != 200)
                    return Error<ProductListDtoSingleResult>(null, message: result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);
            });
        }

        /// <summary>
        /// دریافت اطلاعات یک محصول
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Task<ResponseState<ProductDtoSingleResult>> GetProductById_UI(long? productId)
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                GetProductByIdClient client = new GetProductByIdClient(BaseUrl, httpClient);

                var result = await client.UIAsync(productId);

                if (result.ResultCode != 200)
                    return Error<ProductDtoSingleResult>(null, message: result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);
            });
        }

        /// <summary>
        /// دریافت رنگ های یک محصول
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Task<ResponseState<ProductColorDtoListResult>> GetProductColorList_UI(long? productId)
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                GetProductColorListClient client = new GetProductColorListClient(BaseUrl, httpClient);

                var result = await client.UIAsync(productId);

                if (result.ResultCode != 200)
                    return Error<ProductColorDtoListResult>(null, message: result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);
            });
        }

        /// <summary>
        /// دریافت انواع بسته بندی های یک محصول
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Task<ResponseState<ProductPackingTypeDtoListResult>> GetProductPackingTypeList_UI(long? productId)
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                GetProductPackingTypeListClient client = new GetProductPackingTypeListClient(BaseUrl, httpClient);

                var result = await client.UIAsync(productId);

                if (result.ResultCode != 200)
                    return Error<ProductPackingTypeDtoListResult>(null, message: result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);
            });
        }

        /// <summary>
        /// لیست محصولات مرتبط با یک محصول
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Task<ResponseState<ProductListResult>> GetRelatedProductList_UI(long? productId)
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                GetRelatedProductListClient client = new GetRelatedProductListClient(BaseUrl, httpClient);

                var result = await client.UIAsync(productId);

                if (result.ResultCode != 200)
                    return Error<ProductListResult>(null, message: result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);
            });
        }

        /// <summary>
        /// لیست پارامترهای یک محصول
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Task<ResponseState<ProductParamDtoListResult>> GetProductParamList_UI(long? productId)
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                GetProductParamListClient client = new GetProductParamListClient(BaseUrl, httpClient);

                var result = await client.UIAsync(productId);

                if (result.ResultCode != 200)
                    return Error<ProductParamDtoListResult>(null, message: result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);
            });
        }

        /// <summary>
        /// لیست محصولات یک دسته
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Task<ResponseState<ProductDtoListResult>> GetProductListByCatId_UI(long? productId)
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                GetProductListByCatIdClient client = new GetProductListByCatIdClient(BaseUrl, httpClient);

                var result = await client.UIAsync(productId);

                if (result.ResultCode != 200)
                    return Error<ProductDtoListResult>(null, message: result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);
            });
        }

        /// <summary>
        /// لیست نظرات درباره یک محصول
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Task<ResponseState<ProductCustomerRateDtoListResult>> GetProductCommentList_UI(long? productId)
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                GetProductCommentListClient client = new GetProductCommentListClient(BaseUrl, httpClient);

                var result = await client.UIAsync(productId);

                if (result.ResultCode != 200)
                    return Error<ProductCustomerRateDtoListResult>(null, message: result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);
            });
        }

        /// <summary>
        /// لیست فایل ها/تصاویر/ویدیوهای یک محصول
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Task<ResponseState<ProductImageDtoListResult>> GetImageListByProductId_UI(long? productId)
        {
            return TryCatch(async () =>
            {
                HttpClient httpClient = new HttpClient();

                string BaseUrl = _httpClientFactory.CreateClient("myHttpClient").BaseAddress.AbsoluteUri;

                GetImageListByProductIdClient client = new GetImageListByProductIdClient(BaseUrl, httpClient);

                var result = await client.UIAsync(productId);

                if (result.ResultCode != 200)
                    return Error<ProductImageDtoListResult>(null, message: result.ResultMessage);

                return Success(data: result, message: result.ResultMessage);
            });
        }

    }
}
