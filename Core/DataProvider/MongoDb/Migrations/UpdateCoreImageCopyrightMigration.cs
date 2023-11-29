using MongoDB.Bson;
using MongoDB.Extensions.Migration;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.DataProvider.MongoDb.Migrations
{
	public class UpdateCoreImageCopyrightMigration : IMigration
	{
		public int Version => 2;

		public void Up(BsonDocument document)
		{
			document.Remove("Copyright");
		}

		public void Down(BsonDocument document)
		{
			var x = 1;
		}
	}
}
