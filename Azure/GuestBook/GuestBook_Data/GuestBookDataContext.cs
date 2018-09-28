using System.Linq;

namespace GuestBook_Data
{
	public class GuestBookDataContext : Microsoft.WindowsAzure.StorageClient.TableServiceContext
	{
		public GuestBookDataContext(string baseAddress, Microsoft.WindowsAzure.StorageCredentials credentials)
			: base(baseAddress, credentials)
		{
			
		}

		public IQueryable<GuestBookEntry> GuestBookEntry
		{
			get
			{
				return CreateQuery<GuestBookEntry>("GuestBookEntry");
			}
		}

	}
}
