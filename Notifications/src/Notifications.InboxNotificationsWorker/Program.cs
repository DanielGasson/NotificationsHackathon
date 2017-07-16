using System;
using Common.Constants;
using Common.Queueing;
using Microsoft.ServiceBus.Messaging;

namespace Notifications.InboxNotificationsWorker
{
	internal class Program
	{
		private static void Main()
		{
			var notificationGenerator = new InboxNotificationGenerator();
			var queueService = new AzureQueueService();

			try
			{
				SetConsoleDisplaySetting();
				Console.WriteLine("Inbox Worker started. Awaiting message...");

				var client = queueService.GetQueueClient(QueueName.Notification);

				var messageOptions = new OnMessageOptions
				{
					AutoComplete = true
				};
				messageOptions.ExceptionReceived += (sender, eventArgs) => { Console.WriteLine(eventArgs.Exception.Message); };

				client.OnMessage(message =>
				{
					var customerId = message.GetCustomerId();
					var firstName = message.Properties["FirstName"].ToString();
					var lastName = message.Properties["LastName"].ToString();
					var uniqueKey = message.Properties["UniqueKey"].ToString();

					switch (message.GetNotificationType())
					{
						case MessageType.MonthlyStatement:
							var notificationCreated = notificationGenerator.GenerateMonthlyStatementNotification(customerId, firstName, lastName, uniqueKey);
							if (notificationCreated)
							{
									queueService.SendMessage(QueueName.Sms,
										new MessageProperty("CustomerId", customerId),
										new MessageProperty("FirstName", firstName),
										new MessageProperty("LastName", lastName)
									);
							}
							Console.WriteLine($"Processed Monthly Statement for {Convert.ToInt32(message.Properties["CustomerId"].ToString())}");
							break;
						case MessageType.Contract:
							// todo
							break;
						case MessageType.PaymentOverdue:
							// todo
							break;
					}
				}, messageOptions);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			Console.ReadLine();
		}

		private static void SetConsoleDisplaySetting()
		{
			Console.Title = "Inbox Worker";
			Console.ForegroundColor = ConsoleColor.Green;
			Console.SetWindowSize(Console.WindowWidth / 2, Console.WindowHeight / 2);
		}
	}
}
