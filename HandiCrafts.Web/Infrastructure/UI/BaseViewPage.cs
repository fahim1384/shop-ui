using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using HandiCrafts.Web.Infrastructure.Framework;
using HandiCrafts.Web.Infrastructure.Security;

namespace HandiCrafts.Web.Infrastructure.UI
{
    public abstract class BaseViewPage<TModel> : RazorPage<TModel>
    {
        #region Injected Properties

        [RazorInject]
        public SignInManager SignInManager { get; set; }

        [RazorInject]
        public UserManager UserManager { get; set; }

        //[RazorInject]
        //public IAppContext AppContext { get; set; }

        [RazorInject]
        public ScriptManager Scripts { get; set; }

        [RazorInject]
        public StyleManager Styles { get; set; }

        #endregion

        #region Properties

        public string Title
        {
            get
            {
                return ViewBag.Title;
            }

            set
            {
                ViewBag.Title = value;
            }
        }

        public string RightSidebarTitle
        {
            get
            {
                return ViewBag.RightSidebarTitle;
            }

            set
            {
                ViewBag.RightSidebarTitle = value;
            }
        }

        public bool RightSidebarEnabled
        {
            get
            {
                return ViewBag.RightSidebarEnabled ?? false;
            }

            set
            {
                ViewBag.RightSidebarEnabled = value;
            }
        }

        public bool SecondColumnEnabled
        {
            get
            {
                return ViewBag.SecondColumnEnabled ?? false;
            }

            set
            {
                ViewBag.SecondColumnEnabled = value;
            }
        }

      
        public bool HasNotifications
        {
            get
            {
                var success = TempData["nk.notify.success"] as IList<string>;
                var error = TempData["nk.notify.error"] as IList<string>;

                return success?.Any() == true || error?.Any() == true;
            }
        }

        #endregion
    }
}
