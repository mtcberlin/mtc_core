using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MtcMvcCore.Areas.Admin.Pages.ContentEditor.Models;
using MtcMvcCore.Core.Caching;
using MtcMvcCore.Core.Models;
using MtcMvcCore.Core.Models.PageModels;
using NLog;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.DataProvider.MongoDb
{
	public class MongoDbComponentDataProvider : IComponentDataProvider
	{
		private readonly Logger _logger;
		private readonly IMongoDbDataProvider _dbDataProvider;

		private static string[] complexExcludedTypes = new[] { "decimal", "string", "datetime", "guid" };

		public MongoDbComponentDataProvider(IMongoDbDataProvider dbDataProvider)
		{
			_logger = LogManager.GetCurrentClassLogger();
			_dbDataProvider = dbDataProvider;
		}

		public string CreateDatasource(string type)
		{
			var comId = Guid.NewGuid();
			var componentConfig = SiteConfiguration.GetComponentConfig(type);
			Type t = Type.GetType(componentConfig.EditorModel);
			var inst = Activator.CreateInstance(t);
			t.GetProperty("Id").SetValue(inst, comId);
			_dbDataProvider.Create(inst);
			return comId.ToString();
		}

		public T GetDatasource<T>(string id) where T : class, new()
		{
			return _dbDataProvider.Get<T, Guid>("Id", Guid.Parse(id));
		}

		public dynamic GetDatasource(string id, string typeName)
		{
			return _dbDataProvider.Get<Guid>("_id", Guid.Parse(id), typeName);
		}

		public bool SaveModel<T>(T model, Guid id)
		{
			var updateResult = _dbDataProvider.Update<T>(model, id);
			CacheManager.Clear();
			return updateResult;
		}

		public bool Delete(Guid componentId)
		{
			_dbDataProvider.Delete(componentId);
			return true;
		}
	}
}
