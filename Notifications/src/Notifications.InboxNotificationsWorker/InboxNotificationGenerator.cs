using Common.DataAccess;
using Common.DataAccess.Models;

namespace Notifications.InboxNotificationsWorker
{
	public class InboxNotificationGenerator
	{
		private readonly IAzureStorageRepository _storageRepository;

		public InboxNotificationGenerator()
		{
			// todo IoC
			_storageRepository = new AzureStorageRepository();
		}

		public bool GenerateMonthlyStatementNotification(int customerId, string firstName, string lastName, string uniqueKey)
		{
			var url = $"http://localhost:80/Customer/Download/{uniqueKey}";

			var customerMessage = new CustomerNotification(customerId)
			{
				Title = "Your Monthly Statement is ready",
				Content = GenerateHtmlForMonthlyStatement(firstName, url)
			};

			return _storageRepository.Save(customerMessage);
		}

		private static string GenerateHtmlForMonthlyStatement(string firstName, string url)
		{
			return $"<table> <tbody> <tr> <td>&nbsp;</td> </tr> <tr> <td>&nbsp;</td> </tr> <tr> <td> <div>DEAR {firstName}</div> <div>&nbsp;</div> <div>Your Capital On Tap Monthly statement is ready.&nbsp;</div> <div>Download it&nbsp;<a title=\"downloadms\" href=\"{url}\">here</a></div> <div>&nbsp;</div> <div>Regards</div> <div>&nbsp;</div> <div>Capital On Tap Team</div> </td> </tr> <tr> <td>&nbsp;</td> </tr> </tbody> </table>";
		}
	}
}
