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
			new CustomerMessageViewModel { Title = "Test 1", Body = "Body for test 1", RowKey = new Guid("BE61B744-BB8E-440B-8EDB-872045E136FA") },
			new CustomerMessageViewModel { Title = "Test 2", Body = "Body for test 2", RowKey = new Guid("AD39361E-0532-42D2-B0CA-0ED17DBAAA09") },
			new CustomerMessageViewModel { Title = "Test 3", Body = "Body for test 3", RowKey = new Guid("60404F64-9E81-4604-933C-6471A5E5A2D5") },
			new CustomerMessageViewModel { Title = "Test 4", Body = "Body for test 4", RowKey = new Guid("51EDE11D-C373-48EF-A686-64B06E30F79B") }
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
