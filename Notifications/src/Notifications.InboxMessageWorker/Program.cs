using System;
using System.Configuration;
using System.Net;
using Microsoft.Azure;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Notifications.InboxMessageWorker
{
	class Program
	{
		private const string TableName = "CustomerEmails";

		static void Main(string[] args)
		{
			try
			{
                //// storage client setup
                var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
                var storageClient = storageAccount.CreateCloudTableClient();
                CreateTableIfExists(storageClient, TableName);

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

				    var emailType = message.GetBody<string>().Trim();
				    switch (emailType)
				    {
				        case "MonthlyStatement":
				        {
                            ProcessMonthlyStatementEmail(message, storageClient);
				            break;
				        }
				        case "DayMinus3DD":
				        {
                            ProcessDayMinus3DD(message, storageClient);
				            break;
				        }

                    }
				}, options);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		    Console.ReadLine();
		}

		private static void CreateTableIfExists(CloudTableClient client, string tableName)
		{
			var table = client.GetTableReference(tableName);
			table.CreateIfNotExists();
		}

		private static bool AddEntityToStorage(CloudTableClient client, ITableEntity entity, string tableName)
		{
			var table = client.GetTableReference(tableName);
			var result = table.Execute(TableOperation.Insert(entity));
			return result.HttpStatusCode == (int) HttpStatusCode.NoContent;
		}

		private static void QueueMsgForTextMessage(object customerId, object firstName, object lastName)
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

	    private static void ProcessDayMinus3DD(BrokeredMessage message, CloudTableClient storageClient)
	    {
	        var firstName = message.Properties["FirstName"].ToString();
	        var lastName = message.Properties["LastName"].ToString();
	        var customerId = Convert.ToInt32(message.Properties["CustomerId"].ToString());
	        var customerEmail = new CustomerEmail
	        (
	            customerId: (int)customerId,
	            title: "Your Direct Debit Payment",
	            content: GenerateHtmlForDayMinus3(firstName)
	        );

	        var success = AddEntityToStorage(storageClient, customerEmail, TableName);
	        if (success)
	        {
	            QueueMsgForTextMessage(customerId, firstName, lastName);
	        }
        }

	    private static string GenerateHtmlForMonthlyStatement(string firstName, string url)
	    {
	        return $"<table> <tbody> <tr> <td>&nbsp;</td> </tr> <tr> <td>&nbsp;</td> </tr> <tr> <td> <div>DEAR {firstName}</div> <div>&nbsp;</div> <div>Your Capital On Tap Monthly statement is ready.&nbsp;</div> <div>Download it&nbsp;<a title=\"downloadms\" href=\"{url}\">here</a></div> <div>&nbsp;</div> <div>Regards</div> <div>&nbsp;</div> <div>Capital On Tap Team</div> </td> </tr> <tr> <td>&nbsp;</td> </tr> </tbody> </table>";
	    }

	    private static void ProcessMonthlyStatementEmail(BrokeredMessage message, CloudTableClient storageClient)
	    {
	        var firstName = message.Properties["FirstName"].ToString();
	        var lastName = message.Properties["LastName"].ToString();
	        var customerId = Convert.ToInt32(message.Properties["CustomerId"].ToString());
	        var uniqueKey = message.Properties["UniqueKey"].ToString();

	        var url = $"http://localhost:80/Customer/Download/{uniqueKey}";

            var customerEmail = new CustomerEmail
	        (
	            customerId: (int)customerId,
	            title: "Your Monthly Statement is ready",
	            content: GenerateHtmlForMonthlyStatement(firstName, url)
	        );

	        var success = AddEntityToStorage(storageClient, customerEmail, TableName);
	        if (success)
	        {
	            QueueMsgForTextMessage(customerId, firstName, lastName);
	        }
        }
    }
}
