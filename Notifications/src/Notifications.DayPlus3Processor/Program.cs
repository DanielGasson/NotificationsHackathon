using System;
using System.Configuration;
using Microsoft.ServiceBus.Messaging;

namespace Notifications.DayPlus3Processor
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            var pdfQueueName = "dayplus3queue";
            var client = QueueClient.CreateFromConnectionString(connectionString, pdfQueueName);

            var messageOptions = new OnMessageOptions
            {
                AutoComplete = true
            };
            messageOptions.ExceptionReceived += (sender, eventArgs) =>
            {
                Console.WriteLine($"PDF Queue Error :{eventArgs.Exception.Message}");
            };

            client.OnMessage(message =>
            {

            });
        }
    }
}
