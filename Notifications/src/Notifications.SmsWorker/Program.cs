using System;
using System.Linq;
using Common.DataAccess;
using Common.Queueing;
using Microsoft.ServiceBus.Messaging;

namespace Notifications.SmsWorker
{
	internal class Program
	{
		private const string SmsContent = "You have a new secure message. Log into your account to view it.";

		private static void Main()
		{
			SetConsoleDisplaySetting();
			Console.WriteLine("SMS Worker started. Awaiting message...");

			var smsSender = new SmsSender();
			var queueService = new AzureQueueService();

			var client = queueService.GetQueueClient(QueueName.Sms);
			var messageOptions = new OnMessageOptions
			{
				AutoComplete = true,
				MaxConcurrentCalls = 2,
			};
			messageOptions.ExceptionReceived += (sender, eventArgs) =>
			{
				Console.WriteLine($"SMS Queue Error :{eventArgs.Exception.Message}");
			};

			client.OnMessage(message =>
			{
				var customer = MockDatabase.Customers.First(c => c.Id == message.GetCustomerId());
				smsSender.SendSms(customer.PhoneNumber, SmsContent);
				Console.WriteLine($"Sent sms to customer {customer.FirstName} - Id: {customer.Id}");
			}, messageOptions);
			Console.ReadLine();
		}

		private static void SetConsoleDisplaySetting()
		{
			Console.Title = "SMS Worker";
			Console.ForegroundColor = ConsoleColor.Green;
			Console.SetWindowSize(Console.WindowWidth / 2, Console.WindowHeight / 2);
		}
	}
}
