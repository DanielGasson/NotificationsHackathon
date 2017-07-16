using System.Collections.Generic;
using System.Linq;

namespace Common.Constants
{
	public static class MessageType
	{
		public const string MonthlyStatement = "MonthlyStatement";
		public const string Contract = "Contract";
		public const string PaymentOverdue = "PaymentOverdue";

		private static IEnumerable<string> _messageTypes = new List<string>();

		public static IEnumerable<string> MessageTypes()
		{
			if (_messageTypes.Any())
			{
				return _messageTypes;
			}

			var fields = typeof(MessageType).GetFields();
			_messageTypes = fields.Select(field => (string) field.GetValue(field)).ToList();
			return _messageTypes;
		}
	}
}
