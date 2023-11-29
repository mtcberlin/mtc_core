using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MtcMvcCore.Core.Helper
{
	public static class CoreUtils
	{
		public static Dictionary<string, object> ParseParams(object parameters)
		{
			var dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(parameters))
			{
				object value = property.GetValue(parameters);
				dictionary.Add(property.Name, value);

			}

			return dictionary;

		}

		public static Dictionary<string, object> GetLangInformations(string uri)
		{
			var dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			var refUri = new Uri(uri.TrimEnd('/'));
			var requestPath = refUri.LocalPath;

			if (requestPath.Contains("easy"))
			{
				var splitted = requestPath.Split('/');
				if (splitted[1] == "easy")
				{
					requestPath = requestPath.Replace("/easy", string.Empty);
					dictionary.Add("easy", true);
				}
				else if (splitted[1].Contains("_easy"))
				{
					requestPath = requestPath.Replace("_easy", string.Empty);
					dictionary.Add("easy", true);
				}
				else
				{
					dictionary.Add("easy", false);
				}
			}
			else
			{
				dictionary.Add("easy", false);
			}

			if (SiteConfiguration.PageContextModels.ContainsKey(requestPath))
			{
				var pageContext = SiteConfiguration.PageContextModels[requestPath];
				if (!string.IsNullOrEmpty(pageContext.Language))
				{
					dictionary.Add("lang", pageContext.Language);
				}
				else
				{
					dictionary.Add("lang", Settings.DefaultLanguage);
				}
			}
			else
			{
				dictionary.Add("lang", Settings.DefaultLanguage);
			}

			return dictionary;

		}
	}
}
