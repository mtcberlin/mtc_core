using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Helper
{
	public class LanguageRouteConstraint : IRouteConstraint
	{
		private readonly List<string> _allowedLanguages;


		public LanguageRouteConstraint(IConfiguration appConfiguration)
		{
			var allowedLanguages = appConfiguration.GetSection("Language:allowedLanguages").Get<List<string>>();
			_allowedLanguages = allowedLanguages ?? new List<string>();
		}

		public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
		{
			if (!values.ContainsKey("lang"))
			{
				return false;
			}

			var lang = values["lang"].ToString();

			
			return _allowedLanguages.Count == 0 || _allowedLanguages.Contains(lang);
		}
	}
}
