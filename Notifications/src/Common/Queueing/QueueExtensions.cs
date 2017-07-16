using System.ComponentModel;

namespace Common.Queueing
{
	public static class QueueExtensions
	{
		public static string Description(this QueueName queueName)
		{
			var attributes = (DescriptionAttribute[])queueName.GetType().GetField(queueName.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);

			return attributes.Length > 0
				? attributes[0].Description
				: string.Empty;
		}
	}
}
