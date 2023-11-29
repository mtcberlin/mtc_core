using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Helper
{
	public class RouteValueTransformer : DynamicRouteValueTransformer
	{
		public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
		{

			var requestPath = httpContext.Request.Path.Value == "/" ? "/" : httpContext.Request.Path.Value.TrimEnd('/').ToLower().Replace(' ', '_');
			if (httpContext.Items.ContainsKey("easy"))
			{

				if (httpContext.Items.ContainsKey("lang"))
				{
					requestPath = requestPath.Replace("_easy", string.Empty);
				}
				else
				{
					requestPath = requestPath.Replace("/easy", string.Empty);
				}
			}

			if (SiteConfiguration.PageContextModels.ContainsKey(requestPath))
			{
				var pageContext = SiteConfiguration.PageContextModels[requestPath];
				if (pageContext.Page.Published || httpContext.Request.Query.ContainsKey("preview") && httpContext.Request.Query["preview"].First() == "1")
				{
					values["controller"] = pageContext.Page.Controller ?? "Default";
					values["action"] = pageContext.Page.Action ?? "Index";
				}
			}
			else
			{
				var splitted = requestPath.Split('/');
				var testpath = string.Join('/', splitted.SkipLast(1));
				if (SiteConfiguration.PageContextModels.ContainsKey(testpath))
				{
					var pageContext = SiteConfiguration.PageContextModels[testpath];
					// if (!string.IsNullOrEmpty(pageContext.Language.UrlParameterName))
					// {
					// 	values["controller"] = pageContext.Page.Controller ?? "Default";
					// 	values["action"] = pageContext.Page.Action ?? "Index";
					// 	values[pageContext.Language.UrlParameterName] = splitted.Last();
					// 	if (!string.IsNullOrEmpty(pageContext.Language))
					// 	{
					// 		values["lang"] = pageContext.Language;
					// 	}
					// }
				}
			}

			httpContext.Items["path"] = requestPath;
			//httpContext.Items.Add("path", requestPath);
			return await new ValueTask<RouteValueDictionary>(values);
		}
	}
}
