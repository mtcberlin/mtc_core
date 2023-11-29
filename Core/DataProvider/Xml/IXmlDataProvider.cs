// ReSharper disable once CheckNamespace
using System.IO;

namespace MtcMvcCore.Core.DataProvider.Xml
{
    public interface IXmlDataProvider
    {
	    T GetData<T>(string filepath) where T : class, new();
		void SetData<T>(T model, string filepath);

		MemoryStream AsMemoryStream<T>(T model);
	}

}
