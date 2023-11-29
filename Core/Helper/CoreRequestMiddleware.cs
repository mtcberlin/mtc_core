using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MtcMvcCore.Core.Constants;
using MtcMvcCore.Core.Services;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Helper
{
	public class CoreRequestMiddleware
	{
		private readonly RequestDelegate _next;
		private bool _requireLogin;

		public CoreRequestMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		// IMyScopedService is injected into Invoke
		public async Task Invoke(HttpContext httpContext)
		{

			_requireLogin = false;

			if (CheckRedirect(httpContext))
			{
				return;
			}

			CheckDebugMode(httpContext);
			CheckEditMode(httpContext);
			if (!_requireLogin || httpContext.Request.Path.StartsWithSegments("/admin/login"))
			{
				await _next(httpContext);
			}
		}

		private bool CheckRedirect(HttpContext httpContext)
		{
			var requestPath = httpContext.Request.Path.Value == "/" ? "/" : httpContext.Request.Path.Value.TrimEnd('/');
			if (!SiteConfiguration.PageContextModels.ContainsKey(requestPath)) return false;
			
			var pageContext = SiteConfiguration.PageContextModels[requestPath];
			
			if (pageContext.Page == null || string.IsNullOrEmpty(pageContext.Page.Redirect))
			{
				return false;
			}

			if (pageContext.Page.Redirect.StartsWith("/"))
			{
				httpContext.Response.Redirect(pageContext.Page.Redirect);
				return true;
			}

			if (pageContext.Page.Redirect.StartsWith("{"))
			{
				// var link = _urlService.GetLinkInformationById(pageContext.SiteMapModel.Redirect);
				// httpContext.Response.Redirect(link.Href);
				return true;
			}

			return false;
		}

		private void CheckDebugMode(HttpContext httpContext)
		{
			if (!Settings.AllowDebugMode) return;
			var x = httpContext.User.IsInRole("Administrators");
			if (httpContext.Request.Query.ContainsKey(ApplicationMode.DebugMode) && !httpContext.User.Identity.IsAuthenticated)
			{
				_requireLogin = true;
				httpContext.Response.Redirect($"/admin/login?ReturnUrl={GetReturnUrl(httpContext)}");
				return;
			}

			if (httpContext.Request.Query.ContainsKey(ApplicationMode.DebugMode))
			{
				httpContext.Items[ApplicationMode.DebugMode] = httpContext.Request.Query[ApplicationMode.DebugMode];
			}
			else
			{
				httpContext.Items[ApplicationMode.DebugMode] = false;
			}
		}

		private void CheckEditMode(HttpContext httpContext)
		{
			if (!Settings.AllowEditMode) return;

			if (httpContext.Request.Query.ContainsKey(ApplicationMode.EditMode) && !httpContext.User.Identity.IsAuthenticated)
			{
				_requireLogin = true;
				httpContext.Response.Redirect($"/admin/login?ReturnUrl={GetReturnUrl(httpContext)}");
				return;
			}

			if (httpContext.Request.Query.ContainsKey(ApplicationMode.EditMode))
			{
				httpContext.Items[ApplicationMode.EditMode] = httpContext.Request.Query[ApplicationMode.EditMode];
			}
			else
			{
				httpContext.Items[ApplicationMode.EditMode] = false;
			}
		}

		private string GetReturnUrl(HttpContext httpContext)
		{
			if (httpContext.Request.QueryString.HasValue)
			{
				return WebUtility.UrlEncode($"{httpContext.Request.Path}{httpContext.Request.QueryString}");
			}

			return WebUtility.UrlEncode($"{httpContext.Request.Path}");
		}
	}
}
