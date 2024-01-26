using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MtcMvcCore.Core.Constants;
using MtcMvcCore.Core.Models.Media;
using MtcMvcCore.Core.Models.PageModels;
using MongoDB.Bson.Serialization;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.DataProvider.MongoDb
{

	public class MongoDbDataProvider : IMongoDbDataProvider
	{

		private IMongoDatabase _db;
		private static string[] complexExcludedTypes = new[] { "decimal", "string", "datetime", "guid" };

		public MongoDbDataProvider()
		{
			if (!string.IsNullOrEmpty(Settings.MongoDbConnectionString) && !string.IsNullOrEmpty(Settings.MongoDbStructureDbName))
			{
				var dbClient = new MongoClient(Settings.MongoDbConnectionString + Settings.MongoDbAuthSource);
				_db = dbClient.GetDatabase(Settings.MongoDbStructureDbName);
			}
		}

		public T Get<T, TK>(FieldDefinition<T, TK> field, TK value) where T : class, new()
		{
			try
			{
				var collectionName = GetCollectionName(typeof(T));
				var filter = Builders<T>.Filter.Eq(field, value);

				var collection = _db.GetCollection<T>(collectionName);
				return collection.Find(filter).First<T>();
			}
			catch
			{
				return new T();
			}
		}

		public dynamic Get<TK>(FieldDefinition<object, TK> field, TK value, string typeName)
		{
			try
			{
				var filter = Builders<object>.Filter.Eq(field, value);

				var collection = _db.GetCollection<object>(typeName);
				return collection.Find(filter).First();
			}
			catch
			{
				return null;
			}
		}

		public List<T> Where<T, TK>(FieldDefinition<T, TK> field, TK value) where T : class, new()
		{
			try
			{
				var collectionName = GetCollectionName(typeof(T));
				var filter = Builders<T>.Filter.Eq(field, value);

				var collection = _db.GetCollection<T>(collectionName);
				return collection.Find(filter).ToList<T>();
			}
			catch
			{
				return new List<T>();
			}
		}

		public List<T> WhereRaw<T, TK>(string field, TK value) where T : class, new()
		{
			try
			{
				var collectionName = GetCollectionName(typeof(T));
				var filter = Builders<BsonDocument>.Filter.Eq(field, Guids.WebRoot);

				var collection = _db.GetCollection<BsonDocument>(collectionName);
				var results = collection.Find(filter).ToList<BsonDocument>();
				var result = new List<T>();
				foreach (var v in results)
				{
					result.Add(CreateInstance<T>(v));
				}

				return result;
			}
			catch
			{
				return new List<T>();
			}
		}

		public List<T> WhereNot<T, TK>(FieldDefinition<T, TK> field, TK value) where T : class, new()
		{
			try
			{
				var collectionName = GetCollectionName(typeof(T));
				var fieldExists = Builders<T>.Filter.Exists(field);
				var filter = Builders<T>.Filter.Ne(field, value);

				var combi = Builders<T>.Filter.And(fieldExists, filter);
				var collection = _db.GetCollection<T>(collectionName);
				return collection.Find(combi).ToList<T>();
			}
			catch
			{
				return new List<T>();
			}
		}

		public List<T> All<T>() where T : class, new()
		{
			try
			{
				var collectionName = GetCollectionName(typeof(T));

				var collection = _db.GetCollection<T>(collectionName);
				return collection.Find(Builders<T>.Filter.Empty).ToList<T>();
			}
			catch
			{
				return new List<T>();
			}
		}

		public bool Update<T>(T model, Guid id)
		{
			try
			{
				var collectionName = GetCollectionName(typeof(T));
				var filter = Builders<T>.Filter.Eq("Id", id);

				var collection = _db.GetCollection<T>(collectionName);
				collection.FindOneAndReplace(filter, model);

				return true;
			}
			catch
			{
				return false;
			}
		}

		public async void Create(object model)
		{
			try
			{
				var collectionName = GetCollectionName(model.GetType());

				var collection = _db.GetCollection<object>(collectionName);
				await collection.InsertOneAsync(model);
			}
			catch
			{
				return;
			}
		}

		public async void Delete<T>(Guid id)
		{
			try
			{
				var collectionName = GetCollectionName(typeof(T));
				var filter = Builders<T>.Filter.Eq("Id", id);

				var collection = _db.GetCollection<T>(collectionName);
				await collection.DeleteOneAsync(filter);

			}
			catch
			{
				return;
			}
		}

		public void Delete(Guid id)
		{
			try
			{
				var collectionName = GetCollectionName(typeof(object));

				var filter = Builders<object>.Filter.Eq("_id", id);

				var collection = _db.GetCollection<object>(collectionName);
				var result = collection.DeleteOne(filter);
			}
			catch
			{
				return;
			}
		}

		private string GetCollectionName(Type type)
		{
			if (type == typeof(CoreMediaBase) || type.IsSubclassOf(typeof(CoreMediaBase)))
			{
				return "Media";
			}
			else if (type == typeof(BasePage) || type.IsSubclassOf(typeof(BasePage)) || type == typeof(BaseItem) || type.IsSubclassOf(typeof(BaseItem)))
			{
				return "Content";
			}
			else
			{
				return type.Name;
			}
		}

		private T CreateInstance<T>(BsonDocument doc) where T : class, new()
		{
			dynamic result;
			var calssMap = GetClassNameFromMap(doc.Elements.FirstOrDefault(i => i.Name == "_t").Value.AsString);
			if (calssMap == null)
			{
				result = new T();
			}
			else
			{
				result = calssMap.CreateInstance();
			}

			GetPropertyValue(result, doc);

			return result as T;
		}

		private void GetPropertyValue(object model, BsonDocument doc)
		{
			PropertyInfo[] propInfos = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (var prop in propInfos)
			{
				if (!prop.PropertyType.IsPrimitive && !complexExcludedTypes.Contains(prop.PropertyType.Name.ToLower()))
				{
					try {
					var element = doc.Elements.FirstOrDefault(i => i.Name.ToLower() == prop.Name.ToLower() || i.Name.ToLower() == $"_{prop.Name.ToLower()}").Value;
					if (element == null || element.IsBsonNull) break;
					var value = prop.GetValue(model);
					if (value == null)
					{
						value = Activator.CreateInstance(prop.PropertyType);
					}
					
					if(element.IsBsonArray) {
						SetArrayValues(prop, value, element.AsBsonArray.Values);
					} else {
						GetPropertyValue(value, element.AsBsonDocument);
					}
					
					} catch(Exception) {

					}
				}
				else
				{
					var docValue = doc.Elements.FirstOrDefault(i => i.Name.ToLower() == prop.Name.ToLower() || i.Name.ToLower() == $"_{prop.Name.ToLower()}");
					if (docValue.Value != null && !docValue.Value.IsBsonNull)
					{
						SetValue(prop, model, docValue);
					}
				}
			}
		}

		private void SetArrayValues(PropertyInfo prop, object model, IEnumerable<BsonValue> values) {
			if(!prop.PropertyType.IsPrimitive && !complexExcludedTypes.Contains(prop.PropertyType.Name.ToLower())) {
				foreach(var value in values) {
					var subModel = Activator.CreateInstance(prop.PropertyType.GenericTypeArguments[0]);
					GetPropertyValue(subModel, value.AsBsonDocument);
					model.GetType().GetMethod("Add").Invoke(model, new[] { subModel });
				}
			} else {
				foreach(var value in values) {
					model.GetType().GetMethod("Add").Invoke(model, new[] { value });
				}
			}
			
		}

		private void SetValue(PropertyInfo prop, object model, BsonElement docValue)
		{
			try
			{
				switch (prop.PropertyType.Name)
				{
					case "Guid":
						prop.SetValue(model, docValue.Value.AsGuid);
						break;
					case "Boolean":
						prop.SetValue(model, docValue.Value.AsBoolean);
						break;
					case "Int32":
						prop.SetValue(model, docValue.Value.AsInt32);
						break;
					case "DateTime":
						prop.SetValue(model, docValue.Value.ToUniversalTime());
						break;
					default:
						prop.SetValue(model, docValue.Value.AsString);
						break;
				}
			}
			catch (Exception) { }
		}

		private BsonClassMap GetClassNameFromMap(string name)
		{
			var result = BsonClassMap.GetRegisteredClassMaps().FirstOrDefault(i => i.Discriminator == name);
			return result;
		}

	}

}
