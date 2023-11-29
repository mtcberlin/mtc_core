using LiteDB;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.DataProvider.LiteDb
{
    public interface ILiteDbContext
    {
        public LiteDatabase Database { get; }
    }
}