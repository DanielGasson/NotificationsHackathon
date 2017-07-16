using System;
using System.Linq;
using System.Windows.Forms;
using Common.Constants;
using Common.DataAccess;
using Common.Queueing;

namespace Notifications.MessageGenerator.App
{
	public partial class MainForm : Form
	{
		private readonly IAzureQueueService _queueService;

		public MainForm()
		{
			InitializeComponent();
			_queueService = new AzureQueueService();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			customerComboBox.DataSource = MockDatabase.Customers;
			customerComboBox.DisplayMember = "FirstName";
			customerComboBox.ValueMember = "Id";

			messageTypeComboBox.DataSource = MessageType.MessageTypes();
		}

		private void SubmitButton_Click(object sender, EventArgs e)
		{
			var id = Convert.ToInt32(customerComboBox.SelectedValue);
			var customer = MockDatabase.Customers.First(c => c.Id == id);

			switch (messageTypeComboBox.SelectedItem.ToString())
			{
				case MessageType.MonthlyStatement:
					_queueService.SendMessage(QueueName.Pdf,
						new MessageProperty("CustomerId", customer.Id),
						new MessageProperty("PDFType", MessageType.MonthlyStatement)
					);
					break;

				case MessageType.Contract:
					_queueService.SendMessage(QueueName.Pdf,
						new MessageProperty("CustomerId", customer.Id),
						new MessageProperty("PDFType", MessageType.Contract)
					);
					break;

				case MessageType.PaymentOverdue:
					_queueService.SendMessage(QueueName.Notification,
						new MessageProperty("CustomerId", customer.Id),
						new MessageProperty("NotificationType", MessageType.PaymentOverdue)
					);
					break;
			}
		}
	}
}
