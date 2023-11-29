using System.Collections.Generic;
using LiteDB;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.DataProvider.LiteDb
{

    public class LiteDbDataProvider : ILiteDbDataProvider
    {

        private LiteDatabase _liteDb;

        public LiteDbDataProvider(ILiteDbContext liteDbContext)
        {
            _liteDb = liteDbContext.Database;
        }
        
        /* Get All Entries in Table
         */
        public IEnumerable<T> FindAll<T>(string table)
        {
            IEnumerable<T> all = (IEnumerable<T>)_liteDb.GetCollection<T>(table).FindAll();

            return all;
        }

        public bool Insert<T>(T newEntry)
        {
            return _liteDb.GetCollection<T>("Api").Insert(newEntry);
        }
    }

}
