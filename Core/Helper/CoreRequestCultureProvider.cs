using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace MtcMvcCore.Core.Helper
{
	public class CoreRequestCultureProvider : RequestCultureProvider
	{
		public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
		{
			var requestPath = httpContext.Request.Path.Value == "/" ? "/" : httpContext.Request.Path.Value.TrimEnd('/').ToLower();
			if (requestPath.Contains("easy"))
			{
				var splitted = requestPath.Split('/');
				if (splitted[1] == "easy")
				{
					requestPath = requestPath.Replace("/easy", string.Empty);
					httpContext.Items.Add("easy", true);
				}
				else if (splitted[1].Contains("_easy"))
				{
					requestPath = requestPath.Replace("_easy", string.Empty);
					httpContext.Items.Add("easy", true);
					httpContext.Items.Add("lang", splitted[1].Split("_")[0]);
				}
			} else if(requestPath.StartsWith("/admin/") || requestPath.StartsWith("/api/" ) ) {
				if (httpContext.Request.Query.ContainsKey("lang") && Settings.AllowedLanguage.Contains(httpContext.Request.Query["lang"]))
				{
					httpContext.Items.Add("lang", httpContext.Request.Query["lang"]);
					return new ProviderCultureResult(httpContext.Request.Query["lang"].ToString());
				}
			} else {
				var splitted = requestPath.Split('/');
				if (Settings.AllowedLanguage.Contains(splitted[1]))
				{
					httpContext.Items.Add("lang", splitted[1]);
				}
			}

			if (SiteConfiguration.PageContextModels.ContainsKey(requestPath))
			{
				var pageContext = SiteConfiguration.PageContextModels[requestPath];
				if (!string.IsNullOrEmpty(pageContext.Language))
				{
					return new ProviderCultureResult(pageContext.Language);
				}
			}
			
			await Task.Yield();
			return new ProviderCultureResult(Settings.DefaultLanguage);
		}
	}
}