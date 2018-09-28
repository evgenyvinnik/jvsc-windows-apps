using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace GuestBook_Data
{
	public class GuestBookDataSource
	{
		private static CloudStorageAccount storageAccount;
		private GuestBookDataContext context;

		static GuestBookDataSource()
		{
			storageAccount = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");

			CloudTableClient.CreateTablesFromModel(
				typeof(GuestBookDataContext),
				storageAccount.TableEndpoint.AbsoluteUri,
				storageAccount.Credentials);
		}

		public GuestBookDataSource()
		{
			context = new GuestBookDataContext(storageAccount.TableEndpoint.AbsoluteUri, storageAccount.Credentials)
			          	{
			          		RetryPolicy = RetryPolicies.Retry(3, TimeSpan.FromSeconds(1))
			          	};
		}

		public IEnumerable<GuestBookEntry> GetGuestBookEntries()
		{
			var results = from g in context.GuestBookEntry
						  where g.PartitionKey == DateTime.UtcNow.ToString("MMddyyyy")
						  select g;
			return results;
		}

		public void AddGuestBookEntry(GuestBookEntry newItem)
		{
			context.AddObject("GuestBookEntry", newItem);
			context.SaveChanges();
		}


		public void UpdateImageThumbnail(string partitionKey, string rowKey, string thumbUrl)
		{
			var results = from g in context.GuestBookEntry
						  where g.PartitionKey == partitionKey && g.RowKey == rowKey
						  select g;

			var entry = results.FirstOrDefault();
			entry.ThumbnailUrl = thumbUrl;
			context.UpdateObject(entry);
			context.SaveChanges();
		}

	}
}
