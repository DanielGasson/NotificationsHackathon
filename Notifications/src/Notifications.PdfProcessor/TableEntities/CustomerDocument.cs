using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Notifications.PdfProcessor.TableEntities
{
    public class CustomerDocument : TableEntity
    {
        public CustomerDocument(int customerId, Guid rowKey)
        {
            PartitionKey = customerId.ToString();
            RowKey = rowKey.ToString();
            CustomerId = customerId;
        }

        public CustomerDocument() { }

        public int CustomerId { get; set; }
        public string DocumentType { get; set; }
        public string FileName { get; set; }
        public DateTime Created { get; set; }

    }
}
