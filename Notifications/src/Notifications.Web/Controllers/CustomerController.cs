using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Notifications.Web.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using Microsoft.WindowsAzure.Storage.Table;

namespace Notifications.Web.Controllers
{
	public class CustomerController : Controller// todo rename to CustomersController
	{
		public void Download(string id)
		{
			try
			{
				var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("Microsoft.Storage.ConnectionString"));

				CloudFileClient fileClient = storageAccount.CreateCloudFileClient();

				CloudFileShare fileShare = fileClient.GetShareReference("pdfstorage");
				if (fileShare.Exists())
				{
					SharedAccessFilePolicy sharedAccessFilePolicy = new SharedAccessFilePolicy()
					{
						SharedAccessExpiryTime = DateTime.UtcNow.AddSeconds(20),
						Permissions = SharedAccessFilePermissions.Read
					};

					// generate SAS token based on policy and use to create a new file  
					CloudFileDirectory rootDirectory = fileShare.GetRootDirectoryReference();
					if (rootDirectory.Exists())
					{
						var customerDocument = GetCustomerDocumentDetails(id);
						var aref = rootDirectory.GetFileReference(customerDocument.FileName);

						var token = aref.GetSharedAccessSignature(sharedAccessFilePolicy);
						var url = aref.Uri + token;

						Download(url, customerDocument.FileName);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
			}
			finally
			{
				Console.WriteLine("Enter to exit..");
			}
		}

		private CustomerDocument GetCustomerDocumentDetails(string rowKey)
		{
			var customerDocument = new CustomerDocument();

			try
			{
				var table = GetCustomerDocumentsTable();
				var query = new TableQuery<CustomerDocument>().Where(
					TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey)	
				);

				var customerDocuments = table.ExecuteQuery(query);
				customerDocument = customerDocuments.FirstOrDefault();
			}
			catch (Exception ex)
			{
				// swallow it for now until we decide what we want to do with this
			}

			return customerDocument;
		}

		private CloudTable GetCustomerDocumentsTable()
		{
			var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("Microsoft.Storage.ConnectionString"));
			var tableClient = storageAccount.CreateCloudTableClient();
			var table = tableClient.GetTableReference("CustomerDocument");
			return table;
		}

		private void Download(string url, string fileName)
		{
			//Create a stream for the file
			Stream stream = null;

			//This controls how many bytes to read at a time and send to the client
			int bytesToRead = 10000;

			// Buffer to read bytes in chunk size specified above
			byte[] buffer = new Byte[bytesToRead];

			// The number of bytes read
			try
			{
				//Create a WebRequest to get the file
				HttpWebRequest fileReq = (HttpWebRequest)HttpWebRequest.Create(url);

				//Create a response for this request
				HttpWebResponse fileResp = (HttpWebResponse)fileReq.GetResponse();

				if (fileReq.ContentLength > 0)
					fileResp.ContentLength = fileReq.ContentLength;

				//Get the Stream returned from the response
				stream = fileResp.GetResponseStream();

				// prepare the response to the client. resp is the client Response
				var resp = HttpContext.Response;

				//Indicate the type of data being sent
				resp.ContentType = "application/pdf";

				//Name the file 
				resp.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
				resp.AddHeader("Content-Length", fileResp.ContentLength.ToString());

				int length;
				do
				{
					// Verify that the client is connected.
					if (resp.IsClientConnected)
					{
						// Read data into the buffer.
						length = stream.Read(buffer, 0, bytesToRead);

						// and write it out to the response's output stream
						resp.OutputStream.Write(buffer, 0, length);

						// Flush the data
						resp.Flush();

						//Clear the buffer
						buffer = new Byte[bytesToRead];
					}
					else
					{
						// cancel the download if client has disconnected
						length = -1;
					}
				} while (length > 0); //Repeat until no data is read
			}
			finally
			{
				if (stream != null)
				{
					//Close the input stream
					stream.Close();
				}
			}
		}
	}
}
