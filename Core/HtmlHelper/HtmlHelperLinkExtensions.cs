
// ReSharper disable once CheckNamespace
using System;
using System.Collections.Specialized;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MtcMvcCore.Core.Models;
using MtcMvcCore.Core.Services;

namespace MtcMvcCore.Core.HtmlHelper
{
	public static class HtmlHelperLinkExtensions
	{

		public static IHtmlContent RenderLink(this IHtmlHelper htmlHelper, LinkField linkField, NameValueCollection parameter = null)
		{
			if (linkField == null)
			{
				return new HtmlString(string.Empty);
			}

			var attr = string.Empty;
			if (parameter != null)
			{
				foreach (string param in parameter)
				{
					attr += $" {param}=\"{parameter[param]}\"";
				}
			}

			string linkUrl;
			string linkText;
			linkUrl = linkField.Link;


			if (HtmlHelperExtensions.IsEasyLanguage(htmlHelper) && !string.IsNullOrEmpty(linkField.TextSimple))
			{
				linkText = linkField.TextSimple;
			}
			else
			{
				linkText = string.IsNullOrEmpty(linkField.Text) ? string.Empty : linkField.Text;
			}


			if (linkField.Type == "0")
			{
				LinkInformation linkInfo;
				Guid guidValue;
				bool isValidGuid = Guid.TryParse(linkUrl, out guidValue);

				if (isValidGuid)
				{
					var urlService = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IUrlService>();
					linkInfo = urlService.GetLinkInformationById(guidValue.ToString(), null, HtmlHelperExtensions.IsEasyLanguage(htmlHelper));
					linkText = string.IsNullOrEmpty(linkText) ? linkInfo.Title : linkText;
					linkUrl = linkInfo.Href;
				}
			}

			if (string.IsNullOrEmpty(linkUrl)) {
				return new HtmlString(string.Empty);
			}
			
			//return new HtmlString($"<a href=\"{linkUrl}\" {attr} ><core-speak class='speak' aria-hidden='true'></core-speak><span class='js-speak-content'>{linkText}</span></a>");
			return new HtmlString($"<a href=\"{linkUrl}\" {attr} ><span class='js-speak-content'>{linkText}</span></a>");
		}

		public static IHtmlContent RenderLink(this IHtmlHelper htmlHelper, string value, string linkText = null, NameValueCollection parameter = null)
		{
			if (string.IsNullOrEmpty(value))
			{
				return new HtmlString(string.Empty);
			}

			var attr = string.Empty;
			if (parameter != null)
			{
				foreach (string param in parameter)
				{
					attr += $" {param}=\"{parameter[param]}\"";
				}
			}
			Guid guidValue;
			LinkInformation linkInfo;
			//string linkTextPage;
			bool isValidGuid = Guid.TryParse(value, out guidValue);
			string linkUrl;

			if (isValidGuid)
			{
				var urlService = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IUrlService>();
				linkInfo = urlService.GetLinkInformationById(value, null, HtmlHelperExtensions.IsEasyLanguage(htmlHelper));
				linkText = string.IsNullOrEmpty(linkText) ? linkInfo.Title : linkText;
				linkUrl = linkInfo.Href;
			}
			else
			{
				linkUrl = value;
			}
			//var title = string.IsNullOrEmpty(linkText) ? linkTextPage : linkText;
			return new HtmlString($"<a href=\"{linkUrl}\" {attr} ><core-speak class='speak' aria-hidden='true'></core-speak><span class='js-speak-content'>{linkText}</span></a>");

		}

		public static IDisposable BeginRenderLink(this IHtmlHelper htmlHelper, LinkField link, NameValueCollection parameters = null)
		{
			var urlService = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IUrlService>();
			var attr = string.Empty;
			if (parameters != null)
			{
				foreach (string param in parameters)
				{
					attr += $" {param}=\"{parameters[param]}\"";
				}
			}

			if (!string.IsNullOrEmpty(link.Target))
			{
				attr += $" target=\"{link.Target}\"";
			}

			Guid pageId;
			if (Guid.TryParse(link.Link, out pageId))
			{
				var linkInfo = urlService.GetLinkInformationById(pageId, null, HtmlHelperExtensions.IsEasyLanguage(htmlHelper));
				attr += $" href=\"{linkInfo.Href}\"";
			} else if(!string.IsNullOrEmpty(link.Link)) {
				attr += $" href=\"{link.Link}\"";
			} else {
				return new RenderLinkView(htmlHelper, false);
			}

			var result = new StringBuilder($"<a {attr} >\n");
			htmlHelper.ViewContext.Writer.Write(result.ToString());
			return new RenderLinkView(htmlHelper, true);
		}

		class RenderLinkView : IDisposable
		{
			private IHtmlHelper _helper;
			private bool _closeTag;

			public RenderLinkView(IHtmlHelper helper, bool closeTag)
			{
				_helper = helper;
				_closeTag = closeTag;
			}

			public void Dispose()
			{

				var result = new StringBuilder();
				if (_closeTag)
				{
					result.Append("</a>");
				}
				this._helper.ViewContext.Writer.Write(result.ToString());
			}
		}

	}
}