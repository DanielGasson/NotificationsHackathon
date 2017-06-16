using System;
using System.Configuration;
using System.Net;
using Microsoft.Azure;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Notifications.EmailProcessor
{
	class Program
	{
		private const string TableName = "CustomerEmails";
		private const string EmailContent = "Hi, your monthly statement is ready.";

		static void Main(string[] args)
		{
			try
			{
				//// storage client setup
				//var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
				//var storageClient = storageAccount.CreateCloudTableClient();
				//CreateTableIfExists(storageClient, TableName);

				// queue client setup
				var queueConnectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
				var queueName = "emailgeneratorqueue";
				var queueClient = QueueClient.CreateFromConnectionString(queueConnectionString, queueName);

				var options = new OnMessageOptions
				{
					AutoComplete = true
				};
				options.ExceptionReceived += (sender, eventArgs) => { Console.WriteLine(eventArgs.Exception.Message); };

				queueClient.OnMessage(message =>
				{

				    var emailType = message.GetBody<string>();
				    switch (emailType)
				    {
				        case "MonthlyStatement":
				        {
				            break;
				        }
				        case "DayMinus3DD":
				        {
				            break;
				        }

                    }

					var customerId = message.Properties["customerId"];
					var firstName = message.Properties["firstName"];
					var lastName = message.Properties["lastName"];

					var customerEmail = new CustomerEmail
					(
						customerId: (int) customerId,
						content: EmailContent
					);

					var success = AddEntityToStorage(storageClient, customerEmail, TableName);
					if (success)
					{
						QueueMsg(customerId, firstName, lastName);
					}
				}, options);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private static bool CreateTableIfExists(CloudTableClient client, string tableName)
		{
			var table = client.GetTableReference(TableName);
			return table.CreateIfNotExists();
		}

		private static bool AddEntityToStorage(CloudTableClient client, ITableEntity entity, string tableName)
		{
			var table = client.GetTableReference(tableName);
			var result = table.Execute(TableOperation.Insert(entity));
			return result.HttpStatusCode == (int) HttpStatusCode.NoContent;
		}

		private static void QueueMsg(object customerId, object firstName, object lastName)
		{
			var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
			var pdfQueueName = "smssenderqueue";
			var client = QueueClient.CreateFromConnectionString(connectionString, pdfQueueName);

			var message = new BrokeredMessage();
			message.Properties.Add("CustomerId", customerId);
			message.Properties.Add("FirstName", firstName);
			message.Properties.Add("LastName", lastName);

			client.Send(message);
		}

	    private static string GenerateHtmlForDayMinus3(string firstName)
	    {
	        return
	            $"<table> <tbody> <tr> <td>&nbsp;</td> </tr> <tr> <td>&nbsp;</td> </tr> <tr> <td> <div>DEAR {firstName}</div> <div>&nbsp;</div> <div>Just a reminder that your Direct Debit payment is due to be taken in 3 days</div> <div>&nbsp;</div> <div>Regards</div> <div>&nbsp;</div> <div>Capital On Tap Team</div> </td> </tr> <tr> <td>&nbsp;</td> </tr> </tbody> </table>";
	    }
    }
}
