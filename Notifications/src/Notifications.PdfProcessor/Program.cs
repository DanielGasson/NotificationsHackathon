using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using Microsoft.WindowsAzure.Storage.Table;
using Notifications.PdfProcessor.TableEntities;
using PdfSharp;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace Notifications.PdfProcessor
{
	class Program
	{
		private const string PdfEnginePath = @"C:\Hackathon\Notifications\src\Notifications.PdfProcessor\lib\Select.Html.dep";

		static void Main(string[] args)
		{
			Console.Title = "PDF Worker";
			Console.ForegroundColor = ConsoleColor.Green;
			Console.SetWindowSize(Console.WindowWidth / 2, Console.WindowHeight / 2);
			Console.WriteLine("PDF Worker started. Awaiting message...");

			try
			{
				var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
				var pdfQueueName = "pdfqueue";
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
                    var customerId = message.Properties["CustomerId"];
                    var customer = CustomerDataBaseStub.FirstOrDefault(x => x.Id == (int) customerId);
                    if (customer == null)
                    {
                        throw new Exception("Customer is null");
                    }
                    var htmlContent = GenerateHtmlForPdf(customer.FirstName, customer.LastName);

                    var pdfStream = GeneratePdfStream(htmlContent);
                    if (pdfStream.Length > 0)
                    {
                        var fileName = SavePdfToFileStorage(pdfStream, customer.Id, "MonthlyStatement");
                        var uniqueKey = SaveDocumentRecord(customer, "MonthlyStatement", fileName);

                        var emailGenerationQueued = QueueEmailGeneration(customer, uniqueKey);
                        if (!emailGenerationQueued)
                        {
                            throw new Exception("Could not queue email");
                        }
                        Console.WriteLine("Just processed PDF for Customer {0}", customer.Id);
                    }
                }, messageOptions);

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

		    Console.ReadLine();
		}

	    public static string SaveDocumentRecord(CustomerRecord customer, string documentType, string fileName)
	    {
	        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
	            ConfigurationManager.AppSettings["Microsoft.Storage.ConnectionString"]);

	        CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
		    CreateTableIfNotExists(tableClient, "CustomerDocument");
			CloudTable table = tableClient.GetTableReference("CustomerDocument");

	        var rowKey = Guid.NewGuid();
	        var customerDoc = new CustomerDocument(customer.Id, rowKey);
	        customerDoc.FileName = fileName;
	        customerDoc.Created = DateTime.Now;
	        customerDoc.DocumentType = documentType;

	        TableOperation insertOperation = TableOperation.Insert(customerDoc);
	        table.Execute(insertOperation);

            return rowKey.ToString();
	    }

		private static void CreateTableIfNotExists(CloudTableClient client, string tableName)
		{
			var table = client.GetTableReference(tableName);
			table.CreateIfNotExists();
		}

		private static bool QueueEmailGeneration(CustomerRecord customer, string uniqueKey)
		{
		    try
		    {
		        var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
		        var pdfQueueName = "emailgeneratorqueue";
		        var client = QueueClient.CreateFromConnectionString(connectionString, pdfQueueName);
		        var message = new BrokeredMessage("MonthlyStatement");
		        message.Properties["FirstName"] = customer.FirstName;
		        message.Properties["LastName"] = customer.LastName;
		        message.Properties["CustomerId"] = customer.Id;
		        message.Properties["UniqueKey"] = uniqueKey;
		        client.Send(message);
		    }
		    catch (Exception ex)
		    {
		        Console.WriteLine("Error {0}", ex.Message);
		        return false;
		    }
		    return true;
		}

	    private static MemoryStream GeneratePdfStream(string content)
	    {
            var stream = new MemoryStream();
	        var pdf = PdfGenerator.GeneratePdf(content, PageSize.A4);
            pdf.Save(stream, false);
	        return stream;
	    }

	    private static string SavePdfToFileStorage(MemoryStream fileStream, int customerId, string fileType)
	    {
	        var today = DateTime.Now;
	        var year = today.Year;
	        var month = today.Month;

	        var fileName = $"{year}{month}-{customerId}-{fileType}.pdf";

            var connectionString = ConfigurationManager.AppSettings["Microsoft.Storage.ConnectionString"];
	        CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);
	        CloudFileClient fileClient = account.CreateCloudFileClient();
	        CloudFileShare pdfLocation = fileClient.GetShareReference("pdfstorage");
	        if (pdfLocation.Exists())
	        {
	            var rootDir = pdfLocation.GetRootDirectoryReference();
	            var fileHandle = rootDir.GetFileReference(fileName);
	            fileHandle.Properties.ContentType = "application/pdf";
	            fileStream.Seek(0, SeekOrigin.Begin);
                fileHandle.UploadFromStream(fileStream);
	            return fileName;
	        }
            return String.Empty;
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

	    public static string GenerateHtmlForPdf(string firstname, string lastname)
	    {
	        return
	            $"<!DOCTYPE html> <html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\"> <head> <meta charset=\"utf-8\" /> <title></title> </head> <body> <table> <tbody> <tr> <td>&nbsp;</td> </tr> <tr> <td>&nbsp;</td> </tr> <tr> <td> <div>DEAR {firstname} {lastname} - THIS IS YOUR VERY DETAILED MONTHLY STATEMENT</div> <div>&nbsp;</div> <div>SUMMARY</div> <div>&nbsp;</div> <div>YOU\\\'RE BROKE!!!</div> </td> </tr> <tr> <td>&nbsp;</td> </tr> </tbody> </table> </body> </html>";

        }



    }

   
}
