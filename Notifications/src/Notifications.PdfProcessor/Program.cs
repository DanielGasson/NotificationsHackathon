using System;
using System.Configuration;
using System.IO;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using SelectPdf;

namespace Notifications.PdfProcessor
{
	class Program
	{
		private const string PdfEnginePath =
            @"C:\Hackathon\Notifications\src\Notifications.PdfProcessor\lib\Select.Html.dep";

		static void Main(string[] args)
		{
			try
			{
				// this is gross....
				var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
				var pdfQueueName = "pdfqueue";
				var client = QueueClient.CreateFromConnectionString(connectionString, pdfQueueName);

			    var messageOptions = new OnMessageOptions
			    {
			        AutoComplete = true,
                    MaxConcurrentCalls = 2
			    };
			    messageOptions.ExceptionReceived += (sender, eventArgs) =>
			    {
                    Console.WriteLine($"PDF Queue Error :{eventArgs.Exception.Message}");
			    };

                client.OnMessage(message =>
                {
                    var pdfStream = GeneratePdf(message.GetBody<string>());
                    if (pdfStream.Length > 0)
                    {
                        SavePdfToFileStorage(pdfStream);
                        var emailGenerationQueued = QueueEmailGeneration();
                        if (!emailGenerationQueued)
                        {
                            throw new Exception("Could not queue email");
                        }
                    }
                }, messageOptions);

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

		    Console.ReadLine();
		}

		private static bool QueueEmailGeneration()
		{
		    try
		    {
		        var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
		        var pdfQueueName = "emailgeneratorqueue";
		        var client = QueueClient.CreateFromConnectionString(connectionString, pdfQueueName);
		        var message = new BrokeredMessage("This is an email");
		        client.Send(message);
		    }
		    catch (Exception ex)
		    {
		        Console.WriteLine("Error {0}", ex.Message);
		        return false;
		    }
		    return true;
		}

	    private static MemoryStream GeneratePdf(string content)
	    {
	        var stream = new MemoryStream();
	        // Either manually copy the dep (generator) file into your build folder or specify it this way.
	        GlobalProperties.HtmlEngineFullPath = PdfEnginePath;
	        var converter = new HtmlToPdf();
	        var htmlToConvert = content;
	        PdfDocument signUpEmailDocument = converter.ConvertHtmlString(htmlToConvert);
	        signUpEmailDocument.Save(stream);
	        signUpEmailDocument.Close();
	        return stream;
	    }

	    private static void SavePdfToFileStorage(MemoryStream fileStream)
	    {

            var connectionString = ConfigurationManager.AppSettings["Microsoft.StorageAccount.ConnectionString"];
	        CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);
	        CloudFileClient fileClient = account.CreateCloudFileClient();
	        CloudFileShare pdfLocation = fileClient.GetShareReference("pdfstorage");
	        if (pdfLocation.Exists())
	        {
	            var rootDir = pdfLocation.GetRootDirectoryReference();
	            var fileHandle = rootDir.GetFileReference($"MyPdfFile-{Guid.NewGuid()}.pdf");
	            fileHandle.Properties.ContentType = "application/pdf";
	            fileStream.Seek(0, SeekOrigin.Begin);
                fileHandle.UploadFromStream(fileStream);
	            
	        }
        }
	}
}
