using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Notifications.InboxMessageWorker
{
	public class CustomerEmail : TableEntity
	{
		public CustomerEmail(int customerId, string title, string content)
		{
			this.PartitionKey = customerId.ToString();
			this.RowKey = Guid.NewGuid().ToString();

			CustomerId = customerId;
			Title = title;
			Content = content;
			CreatedDate = DateTime.Now;
		}

		public CustomerEmail()
		{
		}

		public int CustomerId { get; set; }
		public DateTime CreatedDate { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
	}
}
