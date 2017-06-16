using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Notifications.Web.Controllers
{
	public class NotificationsController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		private IEnumerable<string> GetMessagesForUser()
		{
			return new List<string>();
		}
	}
}
