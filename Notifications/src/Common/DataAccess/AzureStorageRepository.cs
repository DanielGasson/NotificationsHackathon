using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using Common.DataAccess.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using Microsoft.WindowsAzure.Storage.Table;

namespace Common.DataAccess
{
	public class AzureStorageRepository : IAzureStorageRepository
	{
		private readonly CloudStorageAccount _account;

		public AzureStorageRepository()
		{
			_account = CloudStorageAccount.Parse(
				ConfigurationManager.AppSettings["Microsoft.Storage.ConnectionString"]);
		}

		public T Get<T>(string property) where T : TableEntity
		{
			throw new NotImplementedException();
		}

		public IEnumerable<T> GetAll<T>() where T : TableEntity
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Save an entity to table storage
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity"></param>
		/// <returns></returns>
		public bool Save<T>(T entity) where T : TableEntity
		{
			var table = GetTableReference(entity);
			CreateTableIfNotExists(table);
			var insertOperation = TableOperation.Insert(entity);
			var result = table.Execute(insertOperation);

			return result.HttpStatusCode == (int) HttpStatusCode.NoContent;
		}

		/// <summary>
		/// Save a stream to file storage
		/// </summary>
		/// <param name="fileStorageName"></param>
		/// <param name="stream">The stream to save to storage</param>
		/// <param name="fileName">The name to give to the stored file</param>
		/// <param name="contentType">The content type the uploaded file should be</param>
		public bool Save(string fileStorageName, MemoryStream stream, string fileName, ContentType contentType)
		{
			var fileStore = GetFileStorageReference(fileStorageName);
			if (fileStore.Exists())
			{
				var rootDirectory = fileStore.GetRootDirectoryReference();
				var fileReference = rootDirectory.GetFileReference(fileName);

				fileReference.Properties.ContentType = contentType.Description();
				stream.Seek(0, SeekOrigin.Begin);
				fileReference.UploadFromStream(stream);

				return true;
			}
			else
			{
				return false;
			}
		}

		private bool CreateTableIfNotExists(CloudTable table)
		{
			return table.CreateIfNotExists();
		}

		/// <summary>
		/// Get a reference to a corresponding entity's table
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		private CloudTable GetTableReference(TableEntity entity)
		{
			var tableName = entity.GetType().Name;
			var client = _account.CreateCloudTableClient();
			return client.GetTableReference(tableName);
		}

		/// <summary>
		/// Get a reference to a file store by name
		/// </summary>
		/// <returns></returns>
		private CloudFileShare GetFileStorageReference(string fileStorageName)
		{
			var fileClient = _account.CreateCloudFileClient();
			var pdfStorageLocation = fileClient.GetShareReference(fileStorageName);

			return pdfStorageLocation;
		}
	}
}
