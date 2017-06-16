using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Notifications.Web.Models;

namespace Notifications.Web.Controllers
{
    public class BaseController : Controller
    {
        public int GetUserId()
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
            return CustomerDb.Customers.FirstOrDefault(x => x.FirstName == ticket.Name).Id;
        }

    }
}