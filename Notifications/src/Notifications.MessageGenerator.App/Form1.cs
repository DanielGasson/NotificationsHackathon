using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.ServiceBus.Messaging;
using Notifications.MessageGenerator.App.Context;

namespace Notifications.MessageGenerator.App
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var customers = CustomerDb.CustomerDataBaseStub;
            comboBox1.DataSource = customers;
            comboBox1.DisplayMember = "FirstName";
            comboBox1.ValueMember = "Id";

            var listOfMessagesType = new List<string> {"MonthlyStatement", "DDEmail"};
            comboBox2.DataSource = listOfMessagesType;
            comboBox2.DisplayMember = "FirstName";

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(comboBox1.SelectedValue.ToString() + " " + comboBox2.SelectedItem);
            var id = Convert.ToInt32(comboBox1.SelectedValue);
            var customer = CustomerDb.CustomerDataBaseStub.FirstOrDefault(x => x.Id == id);
            if (comboBox2.SelectedItem.ToString().Contains("Monthly"))
            {
                GenerateMonthlyStatementMessage(customer);
            }
            else
            {
                GeneratePaymentDueIn3DaysMessage(customer);
            }
        }

        private static void GenerateMonthlyStatementMessage(CustomerDb.CustomerRecord customer)
        {
            var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            var pdfQueueName = "pdfqueue";
            var client = QueueClient.CreateFromConnectionString(connectionString, pdfQueueName);
            var message = new BrokeredMessage();
            message.Properties["CustomerId"] = customer.Id;
            client.Send(message);

        }

        private static void GeneratePaymentDueIn3DaysMessage(CustomerDb.CustomerRecord customer)
        {
            var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            var dayplus3queue = "dayplus3queue";
            var client = QueueClient.CreateFromConnectionString(connectionString, dayplus3queue);
            var message = new BrokeredMessage();
            message.Properties["CustomerId"] = customer.Id;
            client.Send(message);
        }
    }
}
