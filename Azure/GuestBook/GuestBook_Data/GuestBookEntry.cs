using System;
using Microsoft.WindowsAzure.StorageClient;

namespace GuestBook_Data
{
	public class GuestBookEntry : TableServiceEntity
	{
		public GuestBookEntry()
		{
			PartitionKey = DateTime.UtcNow.ToString("MMddyyyy");

			// Row key allows sorting, so we make sure the rows come back in time order.
			RowKey = string.Format("{0:10}_{1}", DateTime.MaxValue.Ticks - DateTime.Now.Ticks, Guid.NewGuid());

		}

		public string Message { get; set; }
		public string GuestName { get; set; }
		public string PhotoUrl { get; set; }
		public string ThumbnailUrl { get; set; }

	}
}
