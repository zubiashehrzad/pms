using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMS.Filters
{
    public class CustomAuthorize : ActionFilterAttribute
    {
        public string permissionEntity { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["User"] == null)
            {
                HttpContext.Current.Response.Redirect("~/User/Login", true);
            }
            else
            {
                if (!string.IsNullOrEmpty(permissionEntity))
                {
                    var user = ((PMS.Models.User)HttpContext.Current.Session["User"]);   //if user is loged in then get user data from sesion
                    var permissions = user.Role.UserPermissions;  //get permissions for role of currently logged in user
                    if (!permissions.Any(p => p.Module.ModuleName == permissionEntity))  //verify permission for current module in url
                    {
                        HttpContext.Current.Response.Redirect("~/User/Login", true);  //go to login page
                    }
                }
            }
            //base.OnActionExecuting(filterContext);
        }
    }
}