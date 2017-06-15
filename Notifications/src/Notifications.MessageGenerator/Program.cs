using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.ServiceBus.Messaging;

namespace Notifications.MessageGenerator
{
    class Program
    {
        static readonly Random RandomListGenerator = new Random();

        static void Main(string[] args)
        {
            Console.WriteLine("Press Enter");
            Console.ReadLine();
            var countMs = 0;
            while (countMs < 3)
            {
                var randomIndex = RandomListGenerator.Next(CustomerDataBaseStub.Count);
                GenerateMonthlyStatementMessage(CustomerDataBaseStub[randomIndex]);
                countMs++;
            }

            //var countPlus3 = 0;
            //while (countPlus3 < 2)
            //{
            //    var randomIndex = RandomListGenerator.Next(CustomerDataBaseStub.Count);
            //    GeneratePaymentDueIn3DaysMessage(CustomerDataBaseStub[randomIndex]);
            //    countPlus3++;
            //}

        }

        private static void GenerateMonthlyStatementMessage(CustomerRecord customer)
        {
            var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            var pdfQueueName = "pdfqueue";
            var client = QueueClient.CreateFromConnectionString(connectionString, pdfQueueName);
            var message = new BrokeredMessage();
            message.Properties["CustomerId"] = customer.Id;
            client.Send(message);

        }

        private static void GeneratePaymentDueIn3DaysMessage(CustomerRecord customer)
        {
            var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            var dayplus3queue = "dayplus3queue";
            var client = QueueClient.CreateFromConnectionString(connectionString, dayplus3queue);
            var message = new BrokeredMessage();
            message.Properties["CustomerId"] = customer.Id;
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
