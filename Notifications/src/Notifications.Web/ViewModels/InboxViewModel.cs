using System.Collections.Generic;

namespace Notifications.Web.ViewModels
{
	public class InboxViewModel
	{
		public IEnumerable<CustomerMessageViewModel> Messages { get; set; }
	}
}
