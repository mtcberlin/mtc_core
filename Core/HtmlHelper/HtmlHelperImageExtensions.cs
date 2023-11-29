using System;
using System.Collections.Specialized;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MtcMvcCore.Core.Services;
using MtcMvcCore.Core.Models.Media;
using MtcMvcCore.Core.Models;
using System.IO;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.HtmlHelper
{
	public static class HtmlHelperImageExtensions
	{

		#region FromString

		public static IHtmlContent RenderImage(this IHtmlHelper htmlHelper, string path, string alt, int width = 0, NameValueCollection parameter = null)
		{
			return RenderImage(htmlHelper, new CoreImage { SavePath = path, Alt = new TextField { Value = alt } }, width, parameter);
		}

		public static IHtmlContent RenderImage(this IHtmlHelper htmlHelper, string path, string alt, string mimeType, int width = 0, int height = 0, NameValueCollection parameter = null)
		{
			return RenderImage(htmlHelper, new CoreImage { SavePath = path, Alt = new TextField { Value = alt }, Type = mimeType }, width, parameter);
		}

		public static IDisposable RenderResponsiveImage(this IHtmlHelper htmlHelper, string imgPath, NameValueCollection imgTagParameter = null, NameValueCollection pictureTagParameter = null)
		{
			CoreImage img = new CoreImage
			{
				SavePath = string.Empty,
				FileName = imgPath
			};
			return RenderResponsiveImage(htmlHelper, img, imgTagParameter, pictureTagParameter);
		}

		public static IHtmlContent RenderResponsiveSource(this IHtmlHelper htmlHelper, string imgPath, int width, int mediaMinWidth)
		{
			var mediaService = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IMediaService>();
			string targetImgPath = mediaService.GetImagePath(imgPath, width);
			string targetImgPath2x = mediaService.GetImagePath2x(imgPath, width);

			var result = new StringBuilder();
			result.Append($"<source srcset='{targetImgPath} {width}w, {targetImgPath2x} {width * 2}w,' media='(min-width: {mediaMinWidth}px)' sizes='{width}px'>\n");

			return new HtmlString(result.ToString());
		}

		#endregion

		#region FromDatabase

		public static IHtmlContent RenderImage(this IHtmlHelper htmlHelper, ImageField imgField, NameValueCollection parameter = null)
		{
			if (imgField == null || imgField.AssetId == Guid.Empty)
			{
				return new HtmlString(string.Empty);
			}
			return RenderImage(htmlHelper, imgField.AssetId, parameter);
		}

		public static IHtmlContent RenderImage(this IHtmlHelper htmlHelper, CoreImage img, NameValueCollection parameter = null)
		{
			if (img == null || string.IsNullOrEmpty(img.SavePath))
			{
				return new HtmlString(string.Empty);
			}
			return RenderImage(htmlHelper, img, img.Width, parameter);
		}

		public static IHtmlContent RenderImage(this IHtmlHelper htmlHelper, Guid imgId, NameValueCollection parameter = null)
		{
			if (imgId == Guid.Empty)
			{
				return new HtmlString(string.Empty);
			}

			var mediaService = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IMediaService>();
			CoreImage img = mediaService.GetImageById(imgId);

			return RenderImage(htmlHelper, img.SavePath, img.Caption.Value, img.Width, parameter);
		}

		public static IHtmlContent RenderImage(this IHtmlHelper htmlHelper, Guid imgId, int width, NameValueCollection parameter = null)
		{
			if (imgId == Guid.Empty)
			{
				return new HtmlString(string.Empty);
			}

			var mediaService = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IMediaService>();
			CoreImage img = mediaService.GetImageById(imgId);

			return RenderImage(htmlHelper, img.SavePath, img.Caption.Value, width, parameter);
		}

		public static IHtmlContent RenderImage(this IHtmlHelper htmlHelper, CoreImage img, int width = 0, NameValueCollection parameter = null)
		{
			if (img == null || string.IsNullOrEmpty(img.SavePath))
			{
				return new HtmlString(string.Empty);
			}

			return RenderImage(htmlHelper, img.SavePath, img.Caption.Value, width, parameter);
		}

		private static HtmlString CreateImageTag(IHtmlHelper htmlHelper, CoreImage img, int width, NameValueCollection parameter = null, bool omitWidthHeight = false)
		{
			var mediaService = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IMediaService>();
			var path = mediaService.GetImagePath(img, width);

			var attr = string.Empty;
			if (parameter != null)
			{
				foreach (string param in parameter)
				{
					attr += $" {param}=\"{parameter[param]}\"";
				}
			}

			if (omitWidthHeight == false)
			{
				if (img.Width > 0)
				{
					attr += $" width=\"{img.Width}\"";
				}

				if (img.Height > 0)
				{
					attr += $" height=\"{img.Height}\"";
				}
			}

			return new HtmlString($"<img src=\"{path}\" alt=\"{img.Alt.Value}\" {attr} />");
		}

		public static IDisposable RenderResponsiveImage(this IHtmlHelper htmlHelper, ImageField imgField, NameValueCollection imgTagParameter = null, NameValueCollection pictureTagParameter = null)
		{
			return RenderResponsiveImage(htmlHelper, imgField.AssetId, imgTagParameter, pictureTagParameter);
		}

		public static IDisposable RenderResponsiveImage(this IHtmlHelper htmlHelper, Guid imgId, NameValueCollection imgTagParameter = null, NameValueCollection pictureTagParameter = null)
		{
			var mediaService = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IMediaService>();
			CoreImage img = mediaService.GetImageById(imgId);
			return RenderResponsiveImage(htmlHelper, img, imgTagParameter, pictureTagParameter);
		}

		public static IDisposable RenderResponsiveImage(this IHtmlHelper htmlHelper, CoreImage img, NameValueCollection imgTagParameter = null, NameValueCollection pictureTagParameter = null)
		{

			var attr = string.Empty;
			if (pictureTagParameter != null)
			{
				foreach (string param in pictureTagParameter)
				{
					attr += $" {param}=\"{pictureTagParameter[param]}\"";
				}
			}

			var result = new StringBuilder($"<picture {attr} >\n");
			htmlHelper.ViewContext.Writer.Write(result.ToString());
			return new ResponsiveImageView(htmlHelper, img, imgTagParameter);
		}

		public static IHtmlContent RenderResponsiveSource(this IHtmlHelper htmlHelper, ImageField imgField, int width, int mediaMinWidth)
		{
			return RenderResponsiveSource(htmlHelper, imgField.AssetId, width, mediaMinWidth);
		}

		public static IHtmlContent RenderResponsiveSource(this IHtmlHelper htmlHelper, Guid imgId, int width, int mediaMinWidth)
		{
			var mediaService = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IMediaService>();
			CoreImage img = mediaService.GetImageById(imgId);

			return RenderResponsiveSource(htmlHelper, img, width, mediaMinWidth);
		}

		public static IHtmlContent RenderResponsiveSource(this IHtmlHelper htmlHelper, CoreImage img, int width, int mediaMinWidth)
		{
			var mediaService = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IMediaService>();
			string imgPath = mediaService.GetImagePath(img, width);
			string imgPath2x = mediaService.GetImagePath2x(img, width);

			var result = new StringBuilder();
			result.Append($"<source srcset='{imgPath} {width}w, {imgPath2x} {width * 2}w,' media='(min-width: {mediaMinWidth}px)' sizes='{width}px'>\n");

			return new HtmlString(result.ToString());
		}

		#endregion

		class ResponsiveImageView : IDisposable
		{
			private IHtmlHelper _helper;
			private CoreImage _img;
			private NameValueCollection _imgTagParameter;

			public ResponsiveImageView(IHtmlHelper helper, CoreImage img, NameValueCollection imgTagParameter = null)
			{
				_helper = helper;
				_imgTagParameter = imgTagParameter;
				_img = img;
			}

			public void Dispose()
			{
				var attr = string.Empty;
				if (_imgTagParameter != null)
				{
					foreach (string param in _imgTagParameter)
					{
						attr += $" {param}=\"{_imgTagParameter[param]}\"";
					}
				}

				var field = _img.Alt ?? new TextField();
				if ((!HtmlHelperExtensions.IsEasyLanguage(_helper) && !string.IsNullOrEmpty(field.Value)) || (HtmlHelperExtensions.IsEasyLanguage(_helper) && string.IsNullOrEmpty(field.SimpleText)))
				{
					attr += $" alt=\"{field.Value}\"";
				}
				if (HtmlHelperExtensions.IsEasyLanguage(_helper) && !string.IsNullOrEmpty(field.SimpleText))
				{
					attr += $" alt=\"{field.SimpleText}\"";
				}

				//TODO:: Das ist doof
				var path = string.IsNullOrEmpty(_img.SavePath) ? Path.Combine("uploads", _img.Id.ToString(), _img.FileName) : Path.Combine(_img.SavePath, _img.FileName);

				var result = new StringBuilder();
				result.Append($"<img src=\"{path}\" {attr} >\n");
				result.Append("</picture>");
				this._helper.ViewContext.Writer.Write(result.ToString());
			}
		}
	}
}