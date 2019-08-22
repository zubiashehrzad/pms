using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Web.Script.Serialization;
using System.Web;

namespace PMS.Filters
{
    public static class CookieReader
    {
        public static PMS.Models.User ReadCookie()
        {
            var myCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];

            // Read the cookie information and display it.
            if (myCookie != null)
            {
                var faTicket = FormsAuthentication.Decrypt(myCookie.Value);
                JavaScriptSerializer seralizer = new JavaScriptSerializer();
                PMS.Models.User obj = seralizer.Deserialize<PMS.Models.User>(faTicket.UserData);
                return obj;
            }
            else
            {
                return new Models.User();
            }

        }
    }
}