using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Notifications.EmailProcessor
{
	public class CustomerEmail : TableEntity
	{
		public CustomerEmail(int customerId, string content)
		{
			this.PartitionKey = customerId.ToString();
			this.RowKey = Guid.NewGuid().ToString();

			CustomerId = customerId;
			Content = content;
			CreatedDate = DateTime.Now;
		}

		public CustomerEmail()
		{
		}

		public int CustomerId { get; set; }
		public DateTime CreatedDate { get; set; }
		public string Content { get; set; }
	}
}
