using System;
using System.Linq;
using Common.Constants;
using Microsoft.ServiceBus.Messaging;

namespace Common.Queueing
{
	public static class MessageExtensions
	{
		public static int GetCustomerId(this BrokeredMessage message)
		{
			return (int) message.Properties["customerId"];
		}

		public static string GetNotificationType(this BrokeredMessage message)
		{
			var notificationType = (string) message.Properties["NotificationType"];

			if (notificationType == null || !MessageType.MessageTypes().Contains(notificationType))
			{
				throw new Exception($"Unrecognised Notification Type {notificationType}");
			}

			return notificationType;
		}
	}
}
