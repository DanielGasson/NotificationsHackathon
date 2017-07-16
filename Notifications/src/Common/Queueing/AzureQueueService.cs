using System;
using System.Configuration;
using Microsoft.ServiceBus.Messaging;

namespace Common.Queueing
{
	public class AzureQueueService : IAzureQueueService
	{
		private readonly string _connectionString;

		public AzureQueueService()
		{
			_connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
		}

		public void SendMessage(QueueName queueName, params MessageProperty[] messageProperties)
		{
			try
			{
				var client = GetClient(queueName);
				var message = new BrokeredMessage();

				foreach (var property in messageProperties)
				{
					message.Properties[property.Key] = property.Value;
				}

				client.Send(message);
			}
			catch (Exception ex)
			{
				// todo - log here
			}
		}

		public QueueClient GetQueueClient(QueueName queueName)
		{
			return GetClient(queueName);
		}

		private QueueClient GetClient(QueueName queueName)
		{
			return QueueClient.CreateFromConnectionString(_connectionString, queueName.Description());
		}
	}
}
