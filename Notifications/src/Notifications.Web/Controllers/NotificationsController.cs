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
			//var message =_messages.First(m => m.RowKey == rowKey);
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
						Body = message.Content
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
					Body = messages.First().Content
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
