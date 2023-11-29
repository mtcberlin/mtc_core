
// ReSharper disable once CheckNamespace
using System;
using MtcMvcCore.Core.Models.PageModels;

namespace MtcMvcCore.Core.DataProvider.Xml
{

    public class XmlComponentDataProvider : IComponentDataProvider 
    {

        public XmlComponentDataProvider()
        {

        }

		public string CreateDatasource(string type)
		{
			throw new NotImplementedException();
		}

		public bool Delete(Guid componentId)
		{
			throw new NotImplementedException();
		}

		public T GetDatasource<T>(string id) where T : class, new()
		{
			throw new NotImplementedException();
		}

		public dynamic GetDatasource(string id, string typeName)
		{
			throw new NotImplementedException();
		}

		public bool SaveModel<T>(T model, Guid id)
		{
			throw new NotImplementedException();
		}
	}

}
