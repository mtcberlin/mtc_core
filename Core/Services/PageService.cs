using System.Linq;
using Microsoft.AspNetCore.Http;
using MtcMvcCore.Core.Models;
using MtcMvcCore.Core.Models.PageModels;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Services
{
	public class ContextService : IContextService
	{
		private readonly IHttpContextAccessor _httpContext;
		private string _lastRequestPath;

		public ContextService(IHttpContextAccessor httpContext)
		{
			_httpContext = httpContext;
		}

		public string CurrentLanguage
		{
			get
			{
				if (_httpContext.HttpContext.Items.ContainsKey("lang"))
				{
					var langParam = _httpContext.HttpContext.Items["lang"].ToString();
					return langParam ?? string.Empty;
				}
				else
				{
					return string.Empty;
				}
			}
		}

		public bool IsEasyLanguage
		{
			get
			{
				if (_httpContext.HttpContext.Items.ContainsKey("easy") && _httpContext.HttpContext.Items["easy"].ToString().ToLower() == "true")
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		public PageContextModel PageContextOverride { get; set; }

		public PageContextModel PageContext
		{
			get
			{
				if (PageContextOverride != null)
				{
					//TODO:: dirty - Sprache muss noch bestimmt werden
					return PageContextOverride;
				}
				else
				{
					_lastRequestPath = string.IsNullOrEmpty(_lastRequestPath) ? GetCurrentRequestPath() : _lastRequestPath;
					return SiteConfiguration.PageContextModels.ContainsKey(_lastRequestPath)
						? SiteConfiguration.PageContextModels[_lastRequestPath]
						: null;
				}
			}
		}

		public PageContextModel RootContext
		{
			get
			{
				return SiteConfiguration.PageContextModels[$"/{CurrentLanguage}"];
			}
		}

		private string GetCurrentRequestPath()
		{
			if (_httpContext.HttpContext.Items.ContainsKey("path"))
			{
				return _httpContext.HttpContext.Items.First(i => (string)i.Key == "path").Value.ToString();
			}

			var result = _httpContext.HttpContext.Request.Path.Value == "/" ? "/" : _httpContext.HttpContext.Request.Path.Value.TrimEnd('/').ToLower();

			var optionalParams = _httpContext.HttpContext.Request.RouteValues.Where(i =>
				!new[] { "controller", "action", "lang", "part" }.Contains(i.Key));
			foreach (var (key, value) in optionalParams)
			{
				result = result.Replace("/" + value, string.Empty).ToLower();
			}

			_lastRequestPath = result;

			return result;
		}
	}
}