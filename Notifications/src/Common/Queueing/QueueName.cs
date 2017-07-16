using System.ComponentModel;

namespace Common.Queueing
{
	public enum QueueName
	{
		[Description("pdfqueue")]
		Pdf,

		[Description("notificationsqueue")]
		Notification,

		[Description("smssenderqueue")]
		Sms
	}
}
