using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Common.DataAccess.Models
{

	public class CustomerNotification : TableEntity
	{
		public int CustomerId { get; set; }
		public DateTime CreatedDate { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }

		public CustomerNotification(int customerId)
		{
			PartitionKey = customerId.ToString();
			RowKey = Guid.NewGuid().ToString();
		}

		public CustomerNotification()
		{
		}
	}
}
