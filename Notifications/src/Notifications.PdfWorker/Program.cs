using System;
using System.IO;
using System.Linq;
using Common.Constants;
using Microsoft.ServiceBus.Messaging;
using PdfSharp;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using Common.DataAccess.Models;
using Common.DataAccess;
using Common.Queueing;

namespace Notifications.PdfWorker
{
	internal class Program
	{
		private static void Main()
		{
			SetConsoleDisplaySetting();
			Console.WriteLine("PDF Worker started. Awaiting message...");

			var queueService = new AzureQueueService();
			var repository = new AzureStorageRepository();

			try
			{
				var client = queueService.GetQueueClient(QueueName.Pdf);

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
					var customerId = message.Properties["CustomerId"];
					var customer = MockDatabase.Customers.FirstOrDefault(c => c.Id == (int) customerId);
					if (customer == null)
					{
						throw new Exception("Customer is null");
					}
					var htmlContent = GenerateHtmlForPdf(customer.FirstName, customer.LastName);

					using (var pdfStream = GeneratePdfStream(htmlContent))
					{
						if (pdfStream.Length > 0)
						{
							// Save file to blob
							var today = DateTime.Now;
							var year = today.Year;
							var month = today.Month;
							var fileName = $"{year}{month}-{customerId}-MonthlyStatement.pdf";
							var success = repository.Save("pdfstorage", pdfStream, fileName, ContentType.Pdf);

							if (success)
							{
								// Save reference to file in table store
								var document = new CustomerDocument(customer.Id, Guid.NewGuid())
								{
									CustomerId = customer.Id,
									FileName = fileName,
									CreatedDate = DateTime.Now,
									DocumentType = "MonthlyStatement"
								};
								repository.Save(document);

								// Queue customer notification
								queueService.SendMessage(QueueName.Notification,
									new MessageProperty("NotificationType", MessageType.MonthlyStatement),
									new MessageProperty("CustomerId", customer.Id),
									new MessageProperty("FirstName", customer.FirstName),
									new MessageProperty("LastName", customer.LastName),
									new MessageProperty("UniqueKey", document.RowKey)
								);
							}
						}
					}

					Console.WriteLine("Just processed PDF for Customer {0}", customer.Id);
				}, messageOptions);

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			Console.ReadLine();
		}

		private static MemoryStream GeneratePdfStream(string content)
		{
			var stream = new MemoryStream();
			var pdf = PdfGenerator.GeneratePdf(content, PageSize.A4);
			pdf.Save(stream, false);
			return stream;
		}

		public static string GenerateHtmlForPdf(string firstname, string lastname)
		{
			return
				$"<!DOCTYPE html> <html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\"> <head> <meta charset=\"utf-8\" /> <title></title> </head> <body> <table> <tbody> <tr> <td>&nbsp;</td> </tr> <tr> <td>&nbsp;</td> </tr> <tr> <td> <div>DEAR {firstname} {lastname} - THIS IS YOUR VERY DETAILED MONTHLY STATEMENT</div> <div>&nbsp;</div> <div>SUMMARY</div> <div>&nbsp;</div> <div>YOU\\\'RE BROKE!!!</div> </td> </tr> <tr> <td>&nbsp;</td> </tr> </tbody> </table> </body> </html>";
		}

		private static void SetConsoleDisplaySetting()
		{
			Console.Title = "PDF Worker";
			Console.ForegroundColor = ConsoleColor.Green;
			Console.SetWindowSize(Console.WindowWidth / 2, Console.WindowHeight / 2);
		}
	}
}
