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
			var message = RetrieveMessage();
			GeneratePdf(message);
		}

		private static string RetrieveMessage()
		{
			var result = string.Empty;

			try
			{
				var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
				var pdfQueueName = "pdfqueue";
				var client = QueueClient.CreateFromConnectionString(connectionString, pdfQueueName);

				var message = client.Receive();
				result = message.GetBody<string>();
				message.Complete();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			return result;
		}

		private static void GeneratePdf(string message)
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
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			Console.ReadKey();
		}
	}
}
