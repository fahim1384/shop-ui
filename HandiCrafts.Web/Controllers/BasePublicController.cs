using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HandiCrafts.Web.Infrastructure.Framework;
using HandiCrafts.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace HandiCrafts.Web.Controllers
{
    public class BasePublicController : BaseController
    {
        public BasePublicController(ILogger logger, IStringLocalizer<SharedResource> localizer, IMapper mapper) : base(logger, localizer, mapper)
        {
        }
    }
}
