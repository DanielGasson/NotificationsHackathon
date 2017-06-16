using System;

namespace Notifications.Web.ViewModels
{
	public class CustomerMessageViewModel
	{
		public int CustomerId { get; set; }
		public Guid RowKey { get; set; }
		public string Title { get; set; }
		public string Body { get; set; }
	}
}
