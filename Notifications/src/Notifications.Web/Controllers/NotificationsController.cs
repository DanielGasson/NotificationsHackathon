using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Notifications.Web.ViewModels;

namespace Notifications.Web.Controllers
{
	public class NotificationsController : Controller
	{
		// mimic db result until I hook up azure storage
		private readonly List<CustomerMessageViewModel> _messages = new List<CustomerMessageViewModel>
		{
			new CustomerMessageViewModel { Title = "Test 1", Body = "Body for test 1", RowKey = Guid.NewGuid() },
			new CustomerMessageViewModel { Title = "Test 2", Body = "Body for test 2", RowKey = Guid.NewGuid() },
			new CustomerMessageViewModel { Title = "Test 3", Body = "Body for test 3", RowKey = Guid.NewGuid() },
			new CustomerMessageViewModel { Title = "Test 4", Body = "Body for test 4", RowKey = Guid.NewGuid() }
		};

		[HttpGet]
		public ActionResult Index()
		{
			var model = new InboxViewModel { Messages = _messages };
			return View(model);
		}

		[HttpGet]
		public ActionResult Message(string id)
		{
			var rowKey = Guid.Parse(id);
			var message =_messages.First(m => m.RowKey == rowKey);
			return PartialView("_Message", message);
		}
	}
}
