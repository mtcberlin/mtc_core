
// ReSharper disable once CheckNamespace
using System;
using MtcMvcCore.Core.Models;
using MtcMvcCore.Core.Models.PageModels;

namespace MtcMvcCore.Core.DataProvider
{
	public interface IComponentDataProvider
	{
		string CreateDatasource(string type);
		bool Delete(Guid componentId);
		T GetDatasource<T>(string id) where T : class, new();
		dynamic GetDatasource(string id, string typeName);
		bool SaveModel<T>(T model, Guid id);
	}
}
