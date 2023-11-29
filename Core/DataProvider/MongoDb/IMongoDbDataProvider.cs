// ReSharper disable once CheckNamespace
using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace MtcMvcCore.Core.DataProvider.MongoDb
{
    public interface IMongoDbDataProvider
    {
	    List<T> All<T>() where T : class, new();
	    List<T> Where<T, TK>(FieldDefinition<T, TK> field, TK value) where T : class, new();
	    List<T> WhereRaw<T, TK>(string field, TK value) where T : class, new();
	    List<T> WhereNot<T, TK>(FieldDefinition<T, TK> field, TK value) where T : class, new();
	    T Get<T, TK>(FieldDefinition<T, TK> field, TK value) where T : class, new();
		dynamic Get<TK>(FieldDefinition<object ,TK> field, TK value, string typeName);
		bool Update<T>(T model, Guid id);
		void Create(object model);
		void Delete<T>(Guid pageId);
		void Delete(Guid id);
	}

}
