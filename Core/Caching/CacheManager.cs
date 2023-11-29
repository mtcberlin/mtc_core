using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using MtcMvcCore.Core.Helper;
using MtcMvcCore.Core.Models;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Caching
{
	public static class CacheManager
	{
		private static MemoryCache _cache;
		private static Dictionary<string, int> TimeBasedCachingKeys { get; set; } = new Dictionary<string, int>();

		public static void Initialize()
		{
			_cache = new MemoryCache(new MemoryCacheOptions());
		}

		public static T Get<T>(string key) where T : class
		{
			return _cache.Get<T>(key);
		}

		public static void Set(string key, object value)
		{
			if (Settings.IsCachingEnabled)
			{
				if (TimeBasedCachingKeys.ContainsKey(key))
				{
					_cache.Set(key, value, TimeSpan.FromSeconds(TimeBasedCachingKeys[key]));
				}
				else
				{
					_cache.Set(key, value);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key">Cache key</param>
		/// <param name="value">Value to Cache</param>
		/// <param name="seconds">Seconds to cache</param>
		public static void Set(string key, object value, int seconds)
		{
			if (Settings.IsCachingEnabled)
			{
				_cache.Set(key, value, TimeSpan.FromSeconds(TimeBasedCachingKeys[key]));
			}
		}

		public static void Remove(string key)
		{
			_cache.Remove(key);
		}

		public static void Clear()
		{
			_cache?.Compact(1.0);
		}

		public static Dictionary<string, string> GatCacheData()
		{
			var result = new Dictionary<string, string>();
			AddMemoryCacheInformations(result);
			
			return result;
		}

		private static void AddMemoryCacheInformations(Dictionary<string, string> result)
		{
			var field = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
			var collection = field.GetValue(_cache) as IEnumerable;

			foreach (var item in collection)
			{
				var keyInfo = item.GetType().GetProperty("Key");
				var key = keyInfo.GetValue(item);

				if (key != null) result.Add(key.ToString(), "HTML");
			}
		}

		public static string BuildCacheKey(string name, object parameters, HttpContext httpContext)
		{
			var cacheKey = string.Empty;
			CachingOptions cachingOptions = null;

			var renderingParameters = parameters as Dictionary<string, object>;
			if (renderingParameters == null)
			{
				renderingParameters = CoreUtils.ParseParams(parameters);
			}

			if (parameters != null && renderingParameters.ContainsKey("Cacheable"))
			{
				cachingOptions = new CachingOptions
				{
					Cacheable = bool.Parse(GetValue(renderingParameters, "Cacheable") ?? "false"),
					VaryByDatasource = bool.Parse(GetValue(renderingParameters, "Cache_VaryByDatasource") ?? "false"),
					VaryBySeoUrl = bool.Parse(GetValue(renderingParameters, "Cache_VaryBySeoUrl") ?? "false"),
					VaryByLogin = bool.Parse(GetValue(renderingParameters, "Cache_VaryByLogin") ?? "false"),
					VaryByQueryString = bool.Parse(GetValue(renderingParameters, "Cache_VaryByQueryString") ?? "false"),
					VaryByDevice = bool.Parse(GetValue(renderingParameters, "Cache_VaryByDevice") ?? "false"),
					VaryByParam = bool.Parse(GetValue(renderingParameters, "Cache_VaryByParam") ?? "false"),
					VaryBySubComponents = bool.Parse(GetValue(renderingParameters, "Cache_VaryBySubComponents") ?? "true"),
					MaxCacheTime = int.Parse(GetValue(renderingParameters, "Cache_MaxCacheTime") ?? "0")
				};
			}
			else
			{
				var componentConfig = SiteConfiguration.GetComponentConfig(name);
				if (componentConfig?.Caching != null)
				{
					cachingOptions = componentConfig.Caching;
				}
			}

			if (cachingOptions == null || !cachingOptions.Cacheable) return cacheKey;

			var cacheKeyList = new List<string> { name };
			if (httpContext.Items.ContainsKey("lang"))
			{
				cacheKeyList.Add(httpContext.Items["lang"].ToString());
			}
			if (httpContext.Items.ContainsKey("easy"))
			{
				cacheKeyList.Add(httpContext.Items["easy"].ToString());
			}
			if (cachingOptions.VaryByDatasource && renderingParameters.ContainsKey("Datasource"))
			{
				cacheKeyList.Add(renderingParameters["Datasource"].ToString());
			}
			if (cachingOptions.VaryBySeoUrl && !string.IsNullOrEmpty(httpContext.Request.Path))
			{
				cacheKeyList.Add(httpContext.Request.Path);
			}
			if (cachingOptions.VaryByView && renderingParameters.ContainsKey("View"))
			{
				cacheKeyList.Add(renderingParameters["View"].ToString());
			}
			if (cachingOptions.VaryByQueryString && !string.IsNullOrEmpty(httpContext.Request.QueryString.Value))
			{
				cacheKeyList.Add(httpContext.Request.QueryString.Value);
			}
			if (cachingOptions.VaryByLogin && httpContext.User?.Identity?.Name != null)
			{
				cacheKeyList.Add(httpContext.User.Identity.Name);
			}
			if (cachingOptions.VaryByDevice && !string.IsNullOrEmpty(httpContext.Request.Headers["User-Agent"]))
			{
				cacheKeyList.Add(httpContext.Request.Headers["User-Agent"].GetHashCode().ToString());
			}
			if (cachingOptions.VaryByParam && parameters != null)
			{
				cacheKeyList.Add(parameters.GetHashCode().ToString());
			}
			if (cachingOptions.VaryBySubComponents && renderingParameters.ContainsKey("SubComponentHash"))
			{
				cacheKeyList.Add(renderingParameters["SubComponentHash"].ToString());
			}

			cacheKey = string.Join('_', cacheKeyList);

			if (cachingOptions.MaxCacheTime > 0 && !TimeBasedCachingKeys.ContainsKey(cacheKey))
			{
				TimeBasedCachingKeys.Add(cacheKey, cachingOptions.MaxCacheTime);
			}

			return cacheKey;

		}

		private static string GetValue(Dictionary<string, object> renderingParameters, string key)
		{
			if (!renderingParameters.ContainsKey(key))
			{
				return null;
			}
			return renderingParameters[key].ToString();
		}

	}
}
