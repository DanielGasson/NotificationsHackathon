using System.Collections.Generic;
using System.IO;
using Common.DataAccess.Models;
using Microsoft.WindowsAzure.Storage.Table;

namespace Common.DataAccess
{
	public interface IAzureStorageRepository
	{
		T Get<T>(string property) where T : TableEntity;

		IEnumerable<T> GetAll<T>() where T : TableEntity;

		bool Save<T>(T entity) where T : TableEntity;

		bool Save(string fileStorageName, MemoryStream stream, string fileName, ContentType contentType);
	}
}
