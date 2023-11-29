using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using MtcMvcCore.Core.Models;
using MtcMvcCore.Core.Models.Media;
using MtcMvcCore.Core.Services;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Helper
{
	public class MediaRequestMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IMediaService _mediaService;

		public MediaRequestMiddleware(RequestDelegate next, IMediaService mediaService)
		{
			_next = next;
			_mediaService = mediaService;
		}

		public async Task Invoke(HttpContext httpContext)
		{
			if (httpContext.Request.Path.StartsWithSegments("/temp"))
			{
				bool ok = _mediaService.CreateImageTransformation(httpContext.Request.Path);
				string contentType;
				new FileExtensionContentTypeProvider().TryGetContentType(httpContext.Request.Path, out contentType);
				httpContext.Response.ContentType = contentType;
				var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", TrimPath(WebUtility.UrlDecode(httpContext.Request.Path)));
				using (var fs = new FileStream(path, FileMode.Open))
				{
					await fs.CopyToAsync(httpContext.Response.Body);
				}
			}
			else
			{
				await _next(httpContext);
			}
		}

		private string TrimPath(string path)
		{
			if (path.Trim().StartsWith("/") || path.Trim().StartsWith("\\"))
			{
				return path.Trim().Substring(1);
			}
			return path;
		}

	}
}
