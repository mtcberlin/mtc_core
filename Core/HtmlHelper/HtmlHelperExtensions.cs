using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MtcMvcCore.Core.Caching;
using MtcMvcCore.Core.Constants;
using MtcMvcCore.Core.Helper;
using MtcMvcCore.Core.Models;
using MtcMvcCore.Core.Services;
using System.Text.RegularExpressions;
using MtcMvcCore.Core.Models.PageModels;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.HtmlHelper
{
	public static class HtmlHelperExtensions
	{

		public static bool IsInEditMode(this IHtmlHelper htmlHelper)
		{
			var isEditMode = false;
			if (htmlHelper.ViewContext.HttpContext.Items.ContainsKey(ApplicationMode.EditMode))
			{
				bool.TryParse(htmlHelper.ViewContext.HttpContext.Items[ApplicationMode.EditMode].ToString(),
					out isEditMode);
			}

			return isEditMode;
		}

		private static bool IsInDebugMode(this IHtmlHelper htmlHelper)
		{
			var isDebugMode = false;
			if (htmlHelper.ViewContext.HttpContext.Items.ContainsKey(ApplicationMode.DebugMode))
			{
				bool.TryParse(htmlHelper.ViewContext.HttpContext.Items[ApplicationMode.DebugMode].ToString(),
					out isDebugMode);
			}

			return isDebugMode;
		}

		public static async Task<IHtmlContent> RenderComponent(this IHtmlHelper htmlHelper, string name,
			object parameter = null)
		{
			var htmlResult = string.Empty;
			var cacheKey = string.Empty;

			var sp = htmlHelper.ViewContext.HttpContext.RequestServices;
			if (!IsInDebugMode(htmlHelper) && !IsInEditMode(htmlHelper) && Settings.IsCachingEnabled)
			{
				cacheKey = CacheManager.BuildCacheKey(name, parameter, htmlHelper.ViewContext.HttpContext);
				if (!string.IsNullOrEmpty(cacheKey))
				{
					htmlResult = CacheManager.Get<string>(cacheKey);
				}

				if (!string.IsNullOrEmpty(htmlResult))
				{
					return new HtmlString(htmlResult);
				}
			}

			AddParamsToViewData(htmlHelper, parameter);

			var helper = new DefaultViewComponentHelper(
				sp.GetRequiredService<IViewComponentDescriptorCollectionProvider>(),
				HtmlEncoder.Default,
				sp.GetRequiredService<IViewComponentSelector>(),
				sp.GetRequiredService<IViewComponentInvokerFactory>(),
				sp.GetRequiredService<IViewBufferScope>());
			await using var writer = new StringWriter();
			var context = htmlHelper.ViewContext;
			helper.Contextualize(context);


			try
			{
				var result = await helper.InvokeAsync(name, parameter);
				result.WriteTo(writer, HtmlEncoder.Default);

				await writer.FlushAsync();
				htmlResult = writer.ToString();
			}
			catch (Exception e)
			{
				var logger = NLog.LogManager.GetCurrentClassLogger();
				logger.Error(e, "Error while render component");
				if (htmlHelper.ViewContext.HttpContext.Items.ContainsKey(ApplicationMode.DebugMode))
				{
					if (IsInDebugMode(htmlHelper))
					{
						cacheKey = string.Empty;
						htmlResult = e.Message + " - " + e.StackTrace;
					}
				}
			}

			if (!string.IsNullOrEmpty(cacheKey))
			{
				CacheManager.Set(cacheKey, htmlResult);
			}

			return new HtmlString(htmlResult);
		}

		private static void AddParamsToViewData(IHtmlHelper htmlHelper, object parameters)
		{
			var renderingParameters = parameters as Dictionary<string, object>;
			if (renderingParameters == null)
			{
				renderingParameters = CoreUtils.ParseParams(parameters);
			}

			foreach (var renderingParameter in renderingParameters)
			{
				htmlHelper.ViewData[renderingParameter.Key] = renderingParameter.Value;
			}
		}

		public static async Task<IHtmlContent> Placeholder(this IHtmlHelper htmlHelper, string placeholderName)
		{
			var contextService = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IContextService>();
			if (contextService.PageContext?.Page?.Placeholders != null)
			{
				var placeholder = contextService.PageContext.Page.Placeholders.FirstOrDefault(i => i.Name == placeholderName);

				if (placeholder == null || placeholder.Components?.Count <= 0)
				{
					return new HtmlString(string.Empty);
				}

				await using var writer = new StringWriter();
				foreach (var component in placeholder.Components)
				{
					var result = await htmlHelper.RenderComponent(component.Name, GetParamObject(component, htmlHelper.ViewContext.HttpContext, placeholderName, contextService.PageContext.Page));
					result.WriteTo(writer, HtmlEncoder.Default);
				}

				await writer.FlushAsync();
				return new HtmlString(writer.ToString());
			}
			else
			{
				return new HtmlString(string.Empty);
			}
		}

		public static async Task<IHtmlContent> SubComponents(this IHtmlHelper htmlHelper)
		{
			var placeholderName = htmlHelper.ViewData["Placeholder"]?.ToString();
			if (string.IsNullOrEmpty(placeholderName)) return new HtmlString(string.Empty);

			var contextService = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IContextService>();
			var placeholder =
				contextService.PageContext.Page.Placeholders.FirstOrDefault(i => i.Name == placeholderName);

			if (placeholder == null || placeholder.Components.Count <= 0)
			{
				return new HtmlString(string.Empty);
			}

			var componentId = htmlHelper.ViewData["ComponentId"]?.ToString();
			if (string.IsNullOrEmpty(componentId)) return new HtmlString(string.Empty);
			var component = placeholder.Components.FirstOrDefault(i => i.ComponentId == componentId);
			await using var writer = new StringWriter();
			foreach (var subComponent in component.Components)
			{
				var result = await htmlHelper.RenderComponent(subComponent.Name, GetParamObject(subComponent, htmlHelper.ViewContext.HttpContext, string.Empty, contextService.PageContext.Page));
				result.WriteTo(writer, HtmlEncoder.Default);
			}

			await writer.FlushAsync();
			return new HtmlString(writer.ToString());

		}

		/// <summary>
		/// Render a TextField as Headline with Value and SimpleText.
		/// Use tag in parameter to override default h2 e.g. {"tag", "h3"}
		/// </summary>
		/// <param>NameValueCollection parameter</param>
		/// <returns>IHtmlContent</returns>
		public static IHtmlContent RenderAsHeadline(this IHtmlHelper htmlHelper, TextField field, NameValueCollection parameter = null)
		{
			if (field == null)
			{
				return new HtmlString(string.Empty);
			}
			var tag = "h2";
			if (parameter != null && !string.IsNullOrEmpty(parameter["tag"]))
			{
				tag = parameter["tag"];
				parameter.Remove("tag");
			}
			var result = new StringBuilder($"<div>\n");
			//TODO:: in function auslagern
			var attr = string.Empty;
			if (parameter != null)
			{
				foreach (string param in parameter)
				{
					attr += $" {param}=\"{parameter[param]}\"";
				}
			}
			if ((!IsEasyLanguage(htmlHelper) && !string.IsNullOrEmpty(field.Value)) || (IsEasyLanguage(htmlHelper) && string.IsNullOrEmpty(field.SimpleText)))
			{
				result.Append($"<{tag} {attr} class='js-speak-content' data-text-type='normal'>\n{field.Value}\n</{tag}>");
			}
			if (IsEasyLanguage(htmlHelper) && !string.IsNullOrEmpty(field.SimpleText))
			{
				result.Append($"<{tag} {attr} class='js-speak-content' data-text-type='simple'>\n{field.SimpleText}\n</{tag}>");
			}
			result.Append($"</div>");
			return new HtmlString(result.ToString());
		}

		/// <summary>
		/// Render a TextField as Text-Block with Value and SimpleText and Video.
		/// Use tag in parameter to override default p e.g. {"tag", "div"}
		/// </summary>
		/// <param>NameValueCollection parameter</param>
		/// <returns>IHtmlContent</returns>
		public static IHtmlContent RenderTextField(this IHtmlHelper htmlHelper, TextField field, NameValueCollection parameter = null)
		{
			if (field == null)
			{
				return new HtmlString(string.Empty);
			}
			var tag = "p";
			if (parameter != null && !string.IsNullOrEmpty(parameter["tag"]))
			{
				tag = parameter["tag"];
				parameter.Remove("tag");
			}

			//TODO:: in function auslagern
			var attr = string.Empty;
			if (parameter != null)
			{
				foreach (string param in parameter)
				{
					attr += $" {param}='{parameter[param]}'";
				}
			}
			var result = new StringBuilder($"<div {attr} >\n");
			if ((!IsEasyLanguage(htmlHelper) && !string.IsNullOrEmpty(field.Value)) || (IsEasyLanguage(htmlHelper) && string.IsNullOrEmpty(field.SimpleText)))
			{
				result.Append($"<div class='js-speak-content' data-text-type='normal'>{field.Value}</div>");
			}

			if (IsEasyLanguage(htmlHelper) && !string.IsNullOrEmpty(field.SimpleText))
			{
				result.Append($"<div class='js-speak-content' data-text-type='simple'>{field.SimpleText}</div>");
			}

			result.Append($"</div>");
			return new HtmlString(result.ToString());
		}

		// /// <summary>
		// /// Render a Toolbar to ...
		// /// </summary>
		// /// <param></param>
		// /// <returns>IHtmlContent</returns>
		// public static IHtmlContent Toolbar(this IHtmlHelper htmlHelper, bool speak)
		// {
		// 	var result = new StringBuilder();
		// 	result.Append($"<div class=\"mb-3 p-1\" style=\"background-color: #eaebe8\">");

		// 	if(speak) {
		// 		result.Append($"<core-speak></core-speak>");
		// 	}

		//     result.Append("</div>");
		// 	return new HtmlString(result.ToString());
		// }

		private static string GetImageDescription(this IHtmlHelper htmlHelper, ImageField imgFld)
		{
			var isEasy = IsEasyLanguage(htmlHelper);

			var desc = string.Empty;
			if (!string.IsNullOrEmpty(imgFld.ImageDescription) && !isEasy)
			{
				desc = imgFld.ImageDescription;
			}
			if (!string.IsNullOrEmpty(imgFld.ImageDescriptionSimple) && isEasy)
			{
				desc = imgFld.ImageDescriptionSimple;
			}

			return desc;
		}

		public static IHtmlContent RenderImageCaption(this IHtmlHelper htmlHelper, ImageField imgFld)
		{

			var isEasy = IsEasyLanguage(htmlHelper);

			var isCaption = false;

			var caption = string.Empty;
			if (!string.IsNullOrEmpty(imgFld.ImageDescription) && !isEasy)
			{
				caption = imgFld.Caption;
				isCaption = true;
			}
			if (!string.IsNullOrEmpty(imgFld.ImageDescriptionSimple) && isEasy)
			{
				caption = imgFld.CaptionSimple;
				isCaption = true;
			}

			if (isCaption)
			{
				return new HtmlString($"<div class='c_img_caption'><core-speak class='speak' aria-hidden='true'></core-speak>{caption}</div>");
			}

			return new HtmlString($"");
		}

		private static Dictionary<string, object> GetParamObject(Component component, HttpContext httpContext, string placeholder, BasePage page)
		{
			var componentParams = string.IsNullOrEmpty(component.Parameters) ? new NameValueCollection() : HttpUtility.ParseQueryString(component.Parameters.Replace(',', '&'));
			var result = new Dictionary<string, object>();
			foreach (string paramKey in componentParams.Keys)
			{
				var value = componentParams.Get(paramKey);
				if (int.TryParse(value, out var intValue))
				{
					result.Add(paramKey, intValue);
				}
				else if (bool.TryParse(value, out var boolValue))
				{
					result.Add(paramKey, boolValue);
				}
				else
				{
					result.Add(paramKey, value);
				}
			}

			//SiteConfiguration.ContentLayouts
			if (!result.ContainsKey("PlaceholderSizes"))
			{
				var layoutConfig = SiteConfiguration.Layouts[page.Layout];
				var viewConfig = SiteConfiguration.ContentLayouts[page.View];
				var placeholderSizes = new Dictionary<string, int>();
				var columnInfo = viewConfig.Placeholders.FirstOrDefault(i => i.Name == placeholder).Columns;
				foreach (var size in layoutConfig.Grid)
				{
					var info = columnInfo?.FirstOrDefault(i => i.Name == size.Name);
					var placeholderColumn = info?.Columns ?? 12;
					placeholderSizes.Add(size.Name, size.MaxWidth / 12 * placeholderColumn);
				}
				result.Add("PlaceholderSizes", placeholderSizes);
			}

			if (!string.IsNullOrEmpty(component.Datasource) && !result.ContainsKey("Datasource"))
			{
				result.Add("Datasource", component.Datasource);
			}

			if (!string.IsNullOrEmpty(component.View) && !result.ContainsKey("View"))
			{
				result.Add("View", component.View);
			}

			if (!string.IsNullOrEmpty(placeholder) && !result.ContainsKey("Placeholder"))
			{
				result.Add("Placeholder", placeholder);
			}

			if (component.Components != null && component.Components.Any())
			{
				result.Add("SubComponentHash", component.Components.GetHashCode());
			}

			result.Add("ComponentId", component.ComponentId);


			if (httpContext.GetRouteData() != null)
			{
				foreach (var routeData in httpContext.GetRouteData().Values)
				{
					if (routeData.Key != "controller" && routeData.Key != "action" && routeData.Key != "area" && !result.ContainsKey(routeData.Key))
					{
						result.Add(routeData.Key, routeData.Value);
					}
				}
			}

			return result;
		}

		public static HtmlString Editable<T>(this IHtmlHelper htmlHelper, T model, Expression<Func<T, object>> field) where T : EditableXmlFileInfo
		{
			var value = GetCompiled(model, field);

			if (htmlHelper.ViewContext.HttpContext.Items.ContainsKey(ApplicationMode.EditMode))
			{
				bool.TryParse(htmlHelper.ViewContext.HttpContext.Items[ApplicationMode.EditMode].ToString(),
					out var isEditMode);
				if (isEditMode)
				{
					var expression = (MemberExpression)field.Body;
					var name = expression.Member.Name;
					if (string.IsNullOrEmpty(value))
					{
						value = "[No text in field]";
					}
					return new HtmlString($"<span class='core-editable' contenteditable='true' data-edit-id='{Guid.NewGuid()}' data-edit-filepath='{WebUtility.UrlEncode(model.FilePath)}' data-edit-fieldname='{name}' data-orginal-value='{WebUtility.UrlEncode(value)}'>{value}</span>");
				}
			}
			// replace link tag
			var urlService = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IUrlService>();
			value = ReplaceLinkIds(value, urlService);

			return new HtmlString(value);
		}

		public static HtmlString CoreLibs(this IHtmlHelper htmlHelper)
		{
			if (IsInEditMode(htmlHelper))
			{
				return new HtmlString("<link rel='stylesheet' href='/admin/css/editMode.min.css' /><script src='/admin/js/editMode.min.js'></script>");
			}

			return new HtmlString(string.Empty);
		}

		public static HtmlString RenderBreakpointDebugInformation(this IHtmlHelper htmlHelper)
		{
			if (Settings.IsDebugInformationEnabled)
			{
				return new HtmlString("<div style=\"position:fixed;color:black;z-index: 100001;top:10px;left:10px;\" class=\"d-block d-sm-none\">XS</div><div style=\"position:fixed;color:black;z-index: 100001;top:10px;left:10px;\" class=\"d-none d-sm-block d-md-none\">SM</div><div style=\"position:fixed;color:black;z-index: 100001;top:10px;left:10px;\" class=\"d-none d-md-block d-lg-none\">MD</div><div style=\"position:fixed;color:black;z-index: 100001;top:10px;left:10px;\" class=\"d-none d-lg-block d-xl-none\">LG</div><div style=\"position:fixed;color:black;z-index: 100001;top:10px;left:10px;\" class=\"d-none d-xl-block\">XL</div>");
			}

			return new HtmlString(string.Empty);
		}

		public static string CurrentLanguage(this IHtmlHelper htmlHelper)
		{
			var contextService = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IContextService>();
			return string.IsNullOrEmpty(contextService.CurrentLanguage) ? Settings.DefaultLanguage : contextService.CurrentLanguage;
		}

		/**
			Temp
		*/
		public static string GetLinkUrlById(this IHtmlHelper htmlHelper, string id)
		{
			var urlService = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IUrlService>();
			return urlService.GetLinkInformationById(id).Href;
		}

		private static string GetCompiled<T>(T model, Expression<Func<T, object>> field)
		{
			return field.Compile()(model).ToString();
		}

		private static string ReplaceLinkIds(string value, IUrlService urlService)
		{
			value = RetrieveGUIDs(value, urlService);
			return value;
		}

		private static string RetrieveGUIDs(string line, IUrlService urlService)
		{

			var matches = Regex.Matches(line, @"{[{{(]?[0-9A-F]{8}[-]?([0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}],?[[a-z]{0,3}}]?");

			//return match.Value;
			foreach (Match match in matches)
			{
				var url = string.Empty;
				if (match.Success)
				{
					var val = match.ToString();
					var valClean = val.Substring(1, val.Length - 2);
					var id = valClean.Split(',');
					if (id.Length == 1)
					{
						var linkInfo = urlService.GetLinkInformationById(id[0]);
						url = linkInfo.Href;
					}
					else
					{
						var linkInfo = urlService.GetLinkInformationById(id[0], id[1]);
						url = linkInfo.Href;
					}

					line = line.Replace(val, url);
				}
			}

			return line;
		}

		public static bool IsEasyLanguage(this IHtmlHelper htmlHelper)
		{
			if (htmlHelper.ViewContext.HttpContext.Items.ContainsKey("easy") && htmlHelper.ViewContext.HttpContext.Items["easy"].ToString().ToLower() == "true")
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}