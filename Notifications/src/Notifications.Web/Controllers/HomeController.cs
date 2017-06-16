using System.Web.Mvc;

namespace Notifications.Web.Controllers
{

	public class HomeController : BaseController
	{
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult About()
		{
		    if (User.Identity.IsAuthenticated)
		    {
		        ViewBag.Message = "Your authenticated." + GetUserId();
		    }
		    else
		    {
		        ViewBag.Message = "You're not authenticated";

		    }
		    return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}

	    public ActionResult Error()
	    {
	        return View("Error");
	    }
	}
}