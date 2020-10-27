using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HandiCrafts.Core.Enums;
using HandiCrafts.Web.Infrastructure.Framework;
using HandiCrafts.Web.Interfaces;
using HandiCrafts.Web.Models.ShoppingCart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace HandiCrafts.Web.Controllers
{
    //[Authorize(Roles = UserRoleNames.User)]
    public class ShoppingCartController : BasePublicController
    {
        public ShoppingCartController(ILogger<ShoppingCartController> logger, IStringLocalizer<SharedResource> localizer, IMapper mapper) : base(logger, localizer, mapper)
        {

        }

        public IActionResult Cart()
        {
            return View();
        }

        public IActionResult Address()
        {
            return View("Address", new AddressContainerModel
            {
                AddressList = new List<AddressModel>()
            {
                new AddressModel()
                {
                    FirstName="اسماعیل",
                    LastName="واحدی",
                    SatatStr="آذربایجانشرقی",
                    CityStr="تبریز",
                    DistrictStr="آبرسانی",
                    SatatId=1,
                    CityId=10,
                    DistrictId=2020,
                    NationalCode="1234567890",
                    Pelak="20",
                    PostalAddress="خیابان امام، کوچه مهرگان اول",
                    RecipientIsSelf=false,
                    PostalCode="1234569870",
                    AptId="2",
                    MobileNo="09148272579",
                }
            }
            });
        }
    }
}
