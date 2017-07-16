using Microsoft.ServiceBus.Messaging;

namespace Common.Queueing
{
	public interface IAzureQueueService
	{
		void SendMessage(QueueName queueName, params MessageProperty[] messageProperties);

		QueueClient GetQueueClient(QueueName queueName);
	}
}
