using LiteDB;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.DataProvider.LiteDb
{
    public class LiteDbContext : ILiteDbContext
    {
        public LiteDatabase Database { get; }

        public LiteDbContext(IOptions<LiteDbOptions> options)
        {
            //Database = new LiteDatabase(options.Value.DatabaseLocation);
            //Database = new LiteDatabase(new ConnectionString(options.Value.DatabaseLocation));
            if (Database == null)
            {
	            Database = new LiteDatabase(new ConnectionString("Data/LiteDb/LiteDbTest.db"));
            }
        }
    }
}
