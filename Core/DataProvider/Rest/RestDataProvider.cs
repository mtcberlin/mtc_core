using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.DataProvider.Rest
{

    public class RestDataProvider : IRestDataProvider
    {
        private readonly HttpClient _client;

        public RestDataProvider(HttpClient client)
        {
            _client = client;
        }


        public async Task<T> GetDataAsync<T>(string url) where T : class, new()
        {
            try
            {
                var httpResponse = await _client.GetAsync(url);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    throw new Exception("Cannot retrieve data");
                }

                var content = await httpResponse.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<T>(content);

                return data;
            }
            catch
            {
                return new T();
            }
        }

    }

}
