using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Notifications.Web.Models;
using Notifications.Web.ViewModels;

namespace Notifications.Web.Controllers
{
	public class NotificationsController : Controller
	{
		private const string TableName = "CustomerEmails";

		[HttpGet]
		public ActionResult Index()
		{
			// just hard code a customer in here since we don't have any auth at the moment
			var messages = GetAllMessagesForCustomer(2);
			var model = new InboxViewModel { Messages = messages };
			return View(model);
		}

		[HttpGet]
		public ActionResult Message(string id)
		{
			var rowKey = Guid.Parse(id);
			var message = GetMessage(rowKey);
			return PartialView("_Message", message);
		}

		private IEnumerable<CustomerMessageViewModel> GetAllMessagesForCustomer(int customerId)
		{
			var result = new List<CustomerMessageViewModel>();
			try
			{
				var table = GetCustomerMessagesTable();

				var query = new TableQuery<CustomerEmail>().Where(
					TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, customerId.ToString())
				);

				var messages = table.ExecuteQuery(query);
				foreach (var message in messages)
				{
					Guid rowKey;
					Guid.TryParse(message.RowKey, out rowKey);

					result.Add(new CustomerMessageViewModel
					{
						CustomerId = message.CustomerId,
						RowKey = rowKey,
						Title = message.Title,
						Body = message.Content,
						Date = message.CreatedDate
					});
				}
			}
			catch (Exception ex)
			{
				// swallow it for now until we decide what we want to do with this
			}

			return result;
		}

		private CustomerMessageViewModel GetMessage(Guid rowKey)
		{
			var message = new CustomerMessageViewModel();
			try
			{
				var table = GetCustomerMessagesTable();

				var query = new TableQuery<CustomerEmail>().Where(
					TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey.ToString())
					);

				var messages = table.ExecuteQuery(query);

				message = new CustomerMessageViewModel
				{
					CustomerId = messages.First().CustomerId,
					RowKey = rowKey,
					Title = messages.First().Title,
					Body = messages.First().Content,
					Date = messages.First().CreatedDate
				};
				
			}
			catch (Exception ex)
			{
				// swallow it for now until we decide what we want to do with this
			}

			return message;
		}

		private CloudTable GetCustomerMessagesTable()
		{
			var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("Microsoft.Storage.ConnectionString"));
			var tableClient = storageAccount.CreateCloudTableClient();
			var table = tableClient.GetTableReference(TableName);
			return table;
		}
	}
}
