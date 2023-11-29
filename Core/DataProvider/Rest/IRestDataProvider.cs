using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.DataProvider.Rest
{
    public interface IRestDataProvider
    {
        Task<T> GetDataAsync<T>(string url) where T : class, new();
    }

}
