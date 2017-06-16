using System;
using System.Configuration;
using System.Linq;
using Microsoft.ServiceBus.Messaging;
using RestSharp;

namespace Notifications.SmsWorker
{
	class Program
	{
		static void Main(string[] args)
		{
			var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
			var pdfQueueName = "smssenderqueue";
			var client = QueueClient.CreateFromConnectionString(connectionString, pdfQueueName);

			var messageOptions = new OnMessageOptions
			{
				AutoComplete = true,
				MaxConcurrentCalls = 2
			};
			messageOptions.ExceptionReceived += (sender, eventArgs) =>
			{
				Console.WriteLine($"SMS Queue Error :{eventArgs.Exception.Message}");
			};

			client.OnMessage(message =>
			{
				var customerId = message.Properties["customerId"];
				var phoneNumber = CustomerDb.Customers.First(c => c.Id == (int) customerId);
				SendSms(phoneNumber.ToString(), "You have a new secure message. Log into your account to view it.");
			}, messageOptions);
		}

		private static void SendSms(string number, string content)
		{
			try
			{
				var apiKey = ConfigurationManager.AppSettings["TextLocalApiKey"];
				var request = new RestRequest("send", Method.POST);
				request.AddParameter("sender", "Dan and Sunit");
				request.AddParameter("message", content);
				request.AddParameter("apiKey", apiKey);
				request.AddParameter("numbers", number);

				var client = new RestClient("https://api.txtlocal.com");
				var response = client.Execute(request);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Failed to send text message. Details: {ex.Message}");
			}
		}
	}
}
