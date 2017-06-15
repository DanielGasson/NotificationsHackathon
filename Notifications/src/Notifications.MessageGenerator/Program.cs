using System.Configuration;
using Microsoft.ServiceBus.Messaging;

namespace Notifications.MessageGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            GenerateMessage();
        }

        private static void GenerateMessage()
        {
            var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            var pdfQueueName = "pdfqueue";
            var client = QueueClient.CreateFromConnectionString(connectionString, pdfQueueName);
            var message = new BrokeredMessage("Hello World!");
            client.Send(message);
        }
    }
}
