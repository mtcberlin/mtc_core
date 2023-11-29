using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.DataProvider.LiteDb
{
    public interface ILiteDbDataProvider
    {
        
        public IEnumerable<T> FindAll<T>(string table);

        //public IAsyncEnumerable<T> FindAllAsync<T>(string table);

        public bool Insert<T>(T newEntry);
    }

}
