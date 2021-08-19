using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace a2.admin.Filters
{
    public class AuthorizeAdminAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var adminName = context.HttpContext.Session.GetString("Name");
            if (adminName != "admin")
                context.Result = new RedirectToActionResult("Index", "Home", null);
        }
        public Boolean isAuthorised(AuthorizationFilterContext context)
        {
            var adminName = context.HttpContext.Session.GetString("Name");
            if (adminName != "admin") { return false; }
            return true;
        }
    }
}
