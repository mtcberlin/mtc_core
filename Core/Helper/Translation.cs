using System;
using System.Collections.Generic;
using System.Resources;
using Microsoft.Extensions.Localization;

namespace MtcMvcCore.Core.Helper
{
	public static class Translation
	{

		private static List<IStringLocalizer> _localicer = new List<IStringLocalizer>();

		internal static void AddLocalizer(IStringLocalizer localicer)
		{
			_localicer.Add(localicer);
		}

		public static string Translate(string name)
		{
			var result = string.Empty;

			foreach(var loc in _localicer) {
				result = loc[name];
				if(result != name) {
					break;
				}
			}

			return result;
		}

		public static IDictionary<string, string> TranslateByKeys(string[] keys){

		IDictionary<string, string> dict = new Dictionary<string, string>();

			foreach(var key in keys){
				dict[key] = Translate(key);
			}
			return dict;
		}
	}
}
