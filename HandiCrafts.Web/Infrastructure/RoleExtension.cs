
using HandiCrafts.Core.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;

namespace Building.Infrastructure
{
    public static class RoleExtension
    {
        public static List<SelectListItem> ConvertRolesToSelectList(RoleManager<Role> roleManager, IStringLocalizer localizer)
        {
            var roles = roleManager.Roles.ToList();
            var selectList = roles.Select(m => new SelectListItem() { Value = m.Name, Text = localizer["UserRoleNames." + m.Name].Value }).ToList();
            return selectList;
        }

        public static string ConvertRolesToLocalizedName(IList<string> roles, IStringLocalizer localizer)
        {
            var localizedRoles = "";
            foreach (var role in roles)
            {
                localizedRoles += localizer["UserRoleNames." + role].Value + ",";
            }

            return localizedRoles.Trim(',');
        }
    }
}
