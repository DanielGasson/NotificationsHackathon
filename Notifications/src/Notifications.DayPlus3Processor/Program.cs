using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.ServiceBus.Messaging;

namespace Notifications.DayPlus3Processor
{
    class Program
    {
        static void Main(string[] args)
        {
			Console.Title = "Direct Debit Message Worker(DDMW)";
			Console.ForegroundColor = ConsoleColor.Green;
			Console.SetWindowSize(Console.WindowWidth / 2, Console.WindowHeight / 2);
			Console.WriteLine("DDMW Worker started. Awaiting message...");

			var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            var pdfQueueName = "dayplus3queue";
            var client = QueueClient.CreateFromConnectionString(connectionString, pdfQueueName);

            var messageOptions = new OnMessageOptions
            {
                AutoComplete = true
            };
            messageOptions.ExceptionReceived += (sender, eventArgs) =>
            {
                Console.WriteLine($"Email Queue Error :{eventArgs.Exception.Message}");
            };

            client.OnMessage(message =>
            {
                var customerId = message.Properties["CustomerId"];
                var customer = CustomerDataBaseStub.FirstOrDefault(x => x.Id == (int)customerId);
                if (customer == null)
                {
                    throw new Exception("Customer is null");
                }
                
                QueueMsgForEmail(customer.Id, customer.FirstName, customer.LastName);
                Console.WriteLine("Queued message for Customer {0}", customer.Id);
            }, messageOptions);

            Console.ReadLine();
        }

        private static void QueueMsgForEmail(int customerId, object firstName, object lastName)
        {
            var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            var pdfQueueName = "emailgeneratorqueue";
            var client = QueueClient.CreateFromConnectionString(connectionString, pdfQueueName);

            var message = new BrokeredMessage("DayMinus3DD");
            message.Properties.Add("CustomerId", customerId);
            message.Properties.Add("FirstName", firstName);
            message.Properties.Add("LastName", lastName);

            client.Send(message);
        }

        public static List<CustomerRecord> CustomerDataBaseStub
        {
            get
            {
                return
                    new List<CustomerRecord>
                    {
                        new CustomerRecord
                        {
                            Id = 1,
                            FirstName = "Sunit",
                            LastName = "Malkan",
                            PhoneNumber = "07507484850"
                        },
                        new CustomerRecord
                        {
                            Id = 2,
                            FirstName = "Daniel",
                            LastName = "Gasson",
                            PhoneNumber = "07949863879"
                        },
                        new CustomerRecord
                        {
                            Id = 3,
                            FirstName = "Jamie",
                            LastName = "Howard",
                            PhoneNumber = "07507484850"
                        },
                        new CustomerRecord
                        {
                            Id = 4,
                            FirstName = "Morgan",
                            LastName = "Faget",
                            PhoneNumber = "07949863879"
                        },
                        new CustomerRecord
                        {
                            Id = 5,
                            FirstName = "Katerina",
                            LastName = "Gerykova",
                            PhoneNumber = "07507484850"
                        }
                    };
            }
        }

        public class CustomerRecord
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string PhoneNumber { get; set; }
        }

    }
}
