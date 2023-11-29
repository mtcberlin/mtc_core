using System;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MtcMvcCore.Core.Services;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.HtmlHelper
{
	public static class HtmlHelperToolbarExtensions
	{
		private static string _svgActiveFillAttribute = "fill='#faa'";

		/// <summary>
		/// Render a Toolbar to ...
		/// </summary>
		/// <param></param>
		/// <returns>IHtmlContent</returns>
		public static IHtmlContent RenderToolbar(this IHtmlHelper htmlHelper, bool speak = false, bool normalLang = false, bool easyLang = false, bool dgsVideo = false)
		{
			var urlService = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IUrlService>();
			var result = new StringBuilder();

			// I un-hid the a11y toolbar. Was there a good reason to hide it?   
			result.Append($"<accessibility-toolbar aria-hidden=\"false\" class=\"d-flex px-2 align-items-center mb-3\" style=\"background-color: #eaebe8;\">");

 			bool colorEasyLang = htmlHelper.IsEasyLanguage();
			if (normalLang)
			{
				var easyLink = urlService.CreateEasyLink(htmlHelper.ViewContext.HttpContext);
				result.Append(GetNormalLangIcon(easyLink, colorEasyLang));
			}

			if (easyLang)
			{
				var easyLink = urlService.CreateEasyLink(htmlHelper.ViewContext.HttpContext);
				result.Append(GetEasyLangIcon(easyLink, colorEasyLang));
			}

			if (speak)
			{
				result.Append($"<core-speak>{GetSpeakIcon()}</core-speak>");
			}

			if (dgsVideo)
			{
				result.Append(GetDgsIcon());
			}

			result.Append($"</accessibility-toolbar>");

			return new HtmlString(result.ToString());
		}

		/// <summary>
		/// Render a Toolbar to ...
		/// </summary>
		/// <param></param>
		/// <returns>IHtmlContent</returns>
		public static IDisposable BeginRenderToolbar(this IHtmlHelper htmlHelper, bool speak = false, bool normalLang = false, bool easyLang = false, bool dgsVideo = false)
		{
			var urlService = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IUrlService>();
			var result = new StringBuilder();

			// I un-hid the a11y toolbar. Was there a good reason to hide it?   
			result.Append($"<accessibility-toolbar aria-hidden=\"false\" class=\"d-flex px-2 align-items-center mb-3\" style=\"background-color: #eaebe8;\">");

            bool colorEasyLang = htmlHelper.IsEasyLanguage();

			if (normalLang)
			{
				var easyLink = urlService.CreateEasyLink(htmlHelper.ViewContext.HttpContext);
				result.Append(GetNormalLangIcon(easyLink, colorEasyLang));
			}

			if (easyLang)
			{
				var easyLink = urlService.CreateEasyLink(htmlHelper.ViewContext.HttpContext);
				result.Append(GetEasyLangIcon(easyLink, colorEasyLang));
			}

			if (speak)
			{
				result.Append($"<core-speak>{GetSpeakIcon()}</core-speak>");
			}

			if (dgsVideo)
			{
				result.Append(GetDgsIcon());
			}

			try
			{
				htmlHelper.ViewContext.Writer.Write(result.ToString());
				return new RenderToolbarView(htmlHelper, true);
			}
			catch (Exception e)
			{
				var logger = NLog.LogManager.GetCurrentClassLogger();
				logger.Error(e, "Error while render Toolbar");
			}

			return new RenderToolbarView(htmlHelper, false);
		}

		class RenderToolbarView : IDisposable
		{
			private IHtmlHelper _helper;
			private bool _closeTag;

			public RenderToolbarView(IHtmlHelper helper, bool closeTag)
			{
				_helper = helper;
				_closeTag = closeTag;
			}

			public void Dispose()
			{

				var result = new StringBuilder();
				if (_closeTag)
				{
					result.Append("</accessibility-toolbar>");
				}
				this._helper.ViewContext.Writer.Write(result.ToString());
			}
		}

		private static string GetNormalLangIcon(string link, bool colorEasyLang)
		{
			string svgProp = "svg xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' width='26' height='26' viewBox='0 0 36.29 30.244'";
			string svgTag = colorEasyLang ? $"<{svgProp}>": $"<{svgProp} {_svgActiveFillAttribute}>";

			var result = new StringBuilder();
			// result.Append("<button class='btn px-2 outline-pink-focus'>");
			result.Append($"<a href='{link}' class='btn px-2 outline-pink-focus' aria-label='Alltagssprache'>");
			result.Append(svgTag);
			result.Append("<defs>");
			result.Append("<clipPath>");
			result.Append("<path data-name='Pfad 116' d='M0-43.4H36.29V-73.641H0Z' transform='translate(0 73.641)'/>");
			result.Append("</clipPath>");
			result.Append("</defs>");
			result.Append("<g data-name='Gruppe 98' clip-path=''>");
			result.Append("<g data-name='Gruppe 97' transform='translate(0 0)'>");
			result.Append("<path data-name='Pfad 115' d='M-52.07-19.467a54.434,54.434,0,0,0-.566-7.763,6.05,6.05,0,0,0-5.3-5.073,103.466,103.466,0,0,0-24.554,0,6.045,6.045,0,0,0-5.3,5.073,54,54,0,0,0,0,15.607,6.043,6.043,0,0,0,5.3,5.08,103.465,103.465,0,0,0,19.541.471,9.184,9.184,0,0,1,4.459.778l5.3,2.416A.766.766,0,0,0-52.155-3.2a.769.769,0,0,0,.088-.358V-19.468Zm-13.606,4.578H-80.8a1.512,1.512,0,0,1-1.551-1.472,1.512,1.512,0,0,1,1.472-1.551h15.2a1.512,1.512,0,0,1,1.551,1.472A1.512,1.512,0,0,1-65.6-14.889Zm6.048-6.048H-80.8a1.512,1.512,0,0,1-1.551-1.472,1.512,1.512,0,0,1,1.472-1.551h21.247a1.512,1.512,0,0,1,1.551,1.472,1.512,1.512,0,0,1-1.472,1.551h-.079Z' transform='translate(88.358 33.035)'/>");
			result.Append("</g>");
			result.Append("</g>");
			result.Append("</svg>");
			result.Append("</a>");
			// result.Append("</button>");

			return result.ToString();
		}

		private static string GetEasyLangIcon(string easyLink, bool colorEasyLang)
		{

			string svgProp = "svg xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' width='26' height='26' viewBox='0 0 34.461 34.561'";
			string svgTag = colorEasyLang ? $"<{svgProp} {_svgActiveFillAttribute}>": $"<{svgProp}>";
			var result = new StringBuilder();
			// result.Append("<button class='btn px-2'>");
			result.Append($"<a href='{easyLink}' class='btn px-2 outline-pink-focus' aria-label='Leichtesprache'>");
			result.Append(svgTag);
			result.Append("<defs>");
			result.Append("<clipPath id='clip-path'>");
			result.Append("<path data-name='Pfad 100' d='M0,11.132H34.461v-34.56H0Z' transform='translate(0 23.428)'/>");
			result.Append("</clipPath>");
			result.Append("</defs>");
			result.Append("<g data-name='Gruppe 75' transform='translate(0 6.799)'>");
			result.Append("<path data-name='Pfad 96' d='M0,7.567l14.6,4.271V-11.652L0-15.924Z' transform='translate(0 15.924)'/>");
			result.Append("</g>");
			result.Append("<g data-name='Gruppe 76' transform='translate(19.864 6.799)'>");
			result.Append("<path data-name='Pfad 97' d='M0,1.376V24.866l14.6-4.272V-2.9Z' transform='translate(0 2.895)'/>");
			result.Append("</g>");
			result.Append("<g data-name='Gruppe 79' clip-path=''>");
			result.Append("<g data-name='Gruppe 77' transform='translate(9.993 -0.001)'>");
			result.Append("<path data-name='Pfad 98' d='M.977,1.954A3.033,3.033,0,0,0,4.01-1.079,3.033,3.033,0,0,0,.977-4.113,3.033,3.033,0,0,0-2.056-1.079,3.033,3.033,0,0,0,.977,1.954' transform='translate(2.056 4.113)'/>");
			result.Append("</g>");
			result.Append("<g data-name='Gruppe 78' transform='translate(17.937 -0.001)'>");
			result.Append("<path data-name='Pfad 99' d='M.977,1.954A3.033,3.033,0,0,0,4.01-1.079,3.033,3.033,0,0,0,.977-4.113,3.033,3.033,0,0,0-2.056-1.079,3.033,3.033,0,0,0,.977,1.954' transform='translate(2.056 4.113)'/>");
			result.Append("</g>");
			result.Append("</g>");
			result.Append("</svg>");
			result.Append("</a>");
			// result.Append("</button>");

			return result.ToString();
		}

    private static string GetSpeakIcon()
    {
      var result = new StringBuilder();
      result.Append("<div class='speak'>");
      result.Append("<button type='button' class='btn px-2 js-start outline-pink-focus' aria-label='Speak Text'>");
      result.Append("<svg xmlns='http://www.w3.org/2000/svg' width='26' height='26' viewBox='0 0 21.555 33.794'>");
      result.Append("<path data-name='Pfad 101' d='M8.344,0-1.55,7.8H-13.211V26H-1.55l9.894,7.8Z' transform='translate(13.211)'/>");
      result.Append("</svg>");
      result.Append("</button>");
      result.Append("<button type='button' class='btn px-1 js-pause d-none outline-pink-focus' aria-label='Pause speak text'>");
      result.Append("<svg xmlns='http://www.w3.org/2000/svg' width='30' height='30' fill='currentColor' class='bi bi-pause-btn' viewBox='0 0 16 16'>");
      result.Append("<path d='M6.25 5C5.56 5 5 5.56 5 6.25v3.5a1.25 1.25 0 1 0 2.5 0v-3.5C7.5 5.56 6.94 5 6.25 5zm3.5 0c-.69 0-1.25.56-1.25 1.25v3.5a1.25 1.25 0 1 0 2.5 0v-3.5C11 5.56 10.44 5 9.75 5z'/><path d='M0 4a2 2 0 0 1 2-2h12a2 2 0 0 1 2 2v8a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2V4zm15 0a1 1 0 0 0-1-1H2a1 1 0 0 0-1 1v8a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1V4z'/>");
      result.Append("</svg>");
      result.Append("</button>");
      result.Append("<button type='button' class='btn px-1 js-play d-none outline-pink-focus' aria-label='Play speak text'>");
      result.Append("<svg xmlns='http://www.w3.org/2000/svg' width='30' height='30' fill='currentColor' class='bi bi-play-btn' viewBox='0 0 16 16'>");
      result.Append("<path d='M6.79 5.093A.5.5 0 0 0 6 5.5v5a.5.5 0 0 0 .79.407l3.5-2.5a.5.5 0 0 0 0-.814l-3.5-2.5z'/><path d='M0 4a2 2 0 0 1 2-2h12a2 2 0 0 1 2 2v8a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2V4zm15 0a1 1 0 0 0-1-1H2a1 1 0 0 0-1 1v8a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1V4z'/>");
      result.Append("</svg>");
      result.Append("</button>");
      result.Append("</div>");

      return result.ToString();
    }

		private static string GetDgsIcon()
		{
			var result = new StringBuilder();
			result.Append("<button class='btn px-2 js-dgs-video-btn outline-pink-focus' aria-label='DGS Video'>");
			result.Append("<svg xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' width='26' height='26' viewBox='0 0 33.938 37.377'>");
			result.Append("<defs>");
			result.Append("<clipPath id='clip-path'>");
			result.Append("<path data-name='Pfad 114' d='M0-53.923H33.938V-91.3H0Z' transform='translate(0 91.3)'/>");
			result.Append("</clipPath>");
			result.Append("</defs>");
			result.Append("<g data-name='Gruppe 95' clip-path=''>");
			result.Append("<g data-name='Gruppe 93' transform='translate(0.075 11.323)'>");
			result.Append("<path data-name='Pfad 112' d='M-20.359,0a1.323,1.323,0,0,0-.383.063,2.5,2.5,0,0,0-.43.182l-.052.028a6.818,6.818,0,0,0-.861.579c-.435.334-.933.763-1.507,1.28l-.4.359c-.383.342-.961.811-1.631,1.355-2.509,2.036-6.3,5.111-7.918,7.868l-.035.054a6.375,6.375,0,0,0-.386.763,6.377,6.377,0,0,0-.507,2.488v4.655c0,3.757,3.119,6.382,7.585,6.382H-7.924a1.716,1.716,0,0,0,1.676-1.726,1.7,1.7,0,0,0-1.676-1.717h-9.587V21.163H-3.694a1.7,1.7,0,0,0,1.676-1.717,1.7,1.7,0,0,0-1.676-1.717h-11V16.28H-2.284A1.7,1.7,0,0,0-.608,14.608v-.093a1.7,1.7,0,0,0-1.676-1.672H-16.1V11.4h11A1.7,1.7,0,0,0-3.422,9.678,1.7,1.7,0,0,0-5.1,7.961H-22.865l0,0,1.918-2.26c1.416-1.629,2.837-3.708,1.279-5.388l-.024-.024A1.118,1.118,0,0,0-19.9.127a.869.869,0,0,0-.24-.1A.914.914,0,0,0-20.359,0' transform='translate(34.471 0)'/>");
			result.Append("</g>");
			result.Append("<g data-name='Gruppe 94' transform='translate(0 0)'>");
			result.Append("<path data-name='Pfad 113' d='M-28.709-34.855l-1.455,0v0h1.448Zm-.637-4.879h-7.58l0,0h7.595l-.012,0m5.272,0h-4.9l-.012,0h4.913ZM-36.149-46.84a13.832,13.832,0,0,0-1.512,1.276l-.4.359c-.382.342-.961.812-1.631,1.355-2.508,2.036-6.3,5.111-7.918,7.867l-.035.054a5.142,5.142,0,0,0-.384.756l0,.01a6.38,6.38,0,0,1,.386-.763l.035-.054c1.618-2.756,5.409-5.832,7.918-7.868.671-.543,1.249-1.013,1.631-1.355l.4-.359c.575-.517,1.073-.946,1.507-1.279m14.843-.043a1.7,1.7,0,0,0-1.217.511l-5,5.274h4.746l2.761-2.9a1.716,1.716,0,0,0-.1-2.4,1.7,1.7,0,0,0-1.183-.477m-13.929-.564h-.007l-.045.028.052-.028m1.277-.118a1.1,1.1,0,0,1,.206.16.976.976,0,0,0-.206-.16m-.491-.133a.972.972,0,0,0-.353.066l0,0a1.324,1.324,0,0,1,.383-.063.935.935,0,0,1,.224.027.981.981,0,0,0-.251-.033m9.946-10a1.7,1.7,0,0,0-1.217.511l-7.86,8.282a2.516,2.516,0,0,1,.837.58c2.6,2.808-.282,6.128-1.228,7.219l-.006.007h4.459l8.863-9.341a1.7,1.7,0,0,0-.092-2.4,1.7,1.7,0,0,0-1.183-.478,1.7,1.7,0,0,0-1.217.511l-7.1,7.486a.714.714,0,0,1-.514.218.715.715,0,0,1-.5-.2h0a.716.716,0,0,1-.038-1.012l8.073-8.507a1.7,1.7,0,0,0-.091-2.4A1.7,1.7,0,0,0-24.5-57.7m-5.437-1.319h-.087A1.7,1.7,0,0,0-31.2-58.5L-43.431-45.613l-.319-2.951c-.269-2.8-1.031-4.613-3.028-4.635-1.376.019-1.324,2.211-1.4,4.642-.061,1.995-.748,7.354-.257,11.328,1.875-2.8,5.458-5.709,7.881-7.676.656-.532,1.228-.993,1.581-1.312l.4-.357a13.707,13.707,0,0,1,2.8-2.121l7.062-7.446a1.705,1.705,0,0,0-.092-2.4,1.7,1.7,0,0,0-1.139-.477' transform='translate(48.609 59.015)'/>");
			result.Append("</g>");
			result.Append("</g>");
			result.Append("</svg>");
			result.Append("</button>");

			return result.ToString();
		}

	}
}