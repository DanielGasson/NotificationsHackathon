using System;
using System.Configuration;
using Microsoft.ServiceBus.Messaging;

namespace Notifications.MessageGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            GeneratePdfMessage();
            ReadMessageQueue();
        }

        private static void GeneratePdfMessage()
        {
            var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            var pdfQueueName = "pdfqueue";
            var client = QueueClient.CreateFromConnectionString(connectionString, pdfQueueName);
            var message = new BrokeredMessage("Hello World!");
            client.Send(message);
        }
        
        private static void ReadMessageQueue()
        {
            var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            var pdfQueueName = "pdfqueue";
            var client = QueueClient.CreateFromConnectionString(connectionString, pdfQueueName);

            client.OnMessage(message =>
            {
                Console.WriteLine($"Message body: {message.GetBody<string>()}");
                Console.WriteLine($"Message id: {message.MessageId}");
            });

            Console.ReadLine();
        }
    }
}
