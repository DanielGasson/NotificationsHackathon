using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Notifications.Web.Models;

namespace Notifications.Web.Controllers
{
    public class AccountController : BaseController
    {
        public ViewResult Login()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Login")]
        public ActionResult PostLogin(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var customer = CustomerDb.Customers.First(x => x.FirstName == loginModel.Name);
                if (customer == null)
                {
                    RedirectToAction("Error", "Home");
                }

                FormsAuthentication.SetAuthCookie(customer.FirstName, true);

                return RedirectToAction("Index", "Notifications");
            }

            return View(loginModel);
        }

        [HttpPost]
        [ActionName("SignOut")]
        public ActionResult PostSignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("index", "home");
        }
    }
}
