
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MtcMvcCore.Core;
using MtcMvcCore.Core.Services;
using MtcMvcCore.SharedComponents.CoreVideoPlayer.Models;
using MtcMvcCore.SharedComponents.CoreVideoPlayer.Service;


// ReSharper disable once CheckNamespace
namespace MtcMvcCore.SharedComponents.CoreVideoPlayer.Controller
{
	[ExcludeFromCodeCoverage]
	public class CoreVideoPlayerViewComponent : ViewComponent
	{
		private readonly ILogger<CoreVideoPlayerViewComponent> _logger;
		private readonly ICoreVideoPlayerService _service;
		private readonly IMediaService _mediaService;

		public CoreVideoPlayerViewComponent(ILogger<CoreVideoPlayerViewComponent> logger, ICoreVideoPlayerService service, IMediaService mediaService)
		{
			_logger = logger;
			_service = service;
			_mediaService = mediaService;
		}

		public async Task<IViewComponentResult> InvokeAsync(string datasource)
		{
			CoreVideoPlayerModel model = _service.GetData(Guid.Parse(datasource));
			
			model.CoreVideo = _mediaService.GetVideo(model.Video.AssetId);

			await Task.Yield();
			return View($"~/{Settings.PathsCorePath}/Components/9_SharedComponents/CoreVideoPlayer/View/Default.cshtml", model);
		}

	}
}