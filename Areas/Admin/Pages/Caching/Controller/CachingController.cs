using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MtcMvcCore.Areas.Admin.Pages.ContentPackages.Controller;
using MtcMvcCore.Core;
using MtcMvcCore.Core.Caching;
using MtcMvcCore.Core.Services;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Areas.Admin.Pages.Caching.Controller
{
	[Authorize(Roles = "Administrator")]
	[ExcludeFromCodeCoverage]
	public class CachingController : AdminBaseController
	{
		private readonly ILogger<CachingController> _logger;
		private readonly IMediaService _mediaService;

		public CachingController(ILogger<CachingController> logger, IMediaService mediaService)
		{
			_logger = logger;
			_mediaService = mediaService;
		}

		[Route("admin/caching")]
		public IActionResult Index(string remove, string type)
		{
			if (!string.IsNullOrEmpty(remove) && remove == "all")
			{
				CacheManager.Clear();
				_mediaService.ClearCache();
			}
			else if (!string.IsNullOrEmpty(remove) && type == "HTML")
			{
				CacheManager.Remove(remove);
			}
			else if (!string.IsNullOrEmpty(remove) && type == "Media")
			{
				_mediaService.Remove(remove);
			}

			if (!string.IsNullOrEmpty(remove))
			{
				Response.Redirect("/admin/caching");
				return View($"~/{Settings.PathsCorePath}/Areas/Admin/Pages/Caching/Views/Caching.cshtml", new Dictionary<string, string>());
			}

			var cacheData = CacheManager.GatCacheData();
			var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp");
			_mediaService.AddMediaCacheInformations(cacheData, path, path);
			return View($"~/{Settings.PathsCorePath}/Areas/Admin/Pages/Caching/Views/Caching.cshtml", cacheData);
		}



	}
}