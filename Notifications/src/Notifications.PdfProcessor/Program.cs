using System;
using System.Configuration;
using Microsoft.ServiceBus.Messaging;
using SelectPdf;

namespace Notifications.PdfProcessor
{
	class Program
	{
		private const string PdfEnginePath = 
			@"E:\Dan\PersonalDev\NotificationsHackathon\NotificationsHackathon\Notifications\src\Notifications.PdfProcessor\lib\Select.Html.dep";
		private const string PdfSavePath = 
			@"E:\Dan\Dev\TestPdf.pdf";

		static void Main(string[] args)
		{
			var result = string.Empty;
			try
			{
				// this is gross....
				var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
				var pdfQueueName = "pdfqueue";
				var client = QueueClient.CreateFromConnectionString(connectionString, pdfQueueName);

				var message = client.Receive();
				result = message.GetBody<string>();

				var success = GeneratePdf(message.GetBody<string>());
				if (success)
				{
					message.Complete();
					var emailGenerationQueued = QueueEmailGeneration();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private static bool QueueEmailGeneration()
		{
			var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
			var pdfQueueName = "emailgeneratorqueue";
			var client = QueueClient.CreateFromConnectionString(connectionString, pdfQueueName);
			var message = new BrokeredMessage("This is an email");
			client.Send(message);
			return true;
		}

		private static bool GeneratePdf(string message)
		{
			try
			{
				// Either manually copy the dep (generator) file into your build folder or specify it this way.
				GlobalProperties.HtmlEngineFullPath = PdfEnginePath;

				var converter = new HtmlToPdf();

				var htmlToConvert = message;

				PdfDocument signUpEmailDocument = converter.ConvertHtmlString(htmlToConvert);
				signUpEmailDocument.Save(PdfSavePath);
				signUpEmailDocument.Close();
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}
	}
}
