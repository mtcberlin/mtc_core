using Microsoft.AspNetCore.Mvc;
using MtcMvcCore.Core.Services;
using System;
using MtcMvcCore.SharedComponents.CoreVideoPlayer.Service;
using MtcMvcCore.SharedComponents.CoreVideoPlayer.Models;
using MtcMvcCore.Core;
using MtcMvcCore.Core.Controllers;

namespace MtcMvcCore.SharedComponents.CoreVideoPlayer.Controller
{
	public class CoreVideoPlayerController : CoreBaseController
	{
		private readonly ICoreVideoPlayerService _service;
		private readonly IContextService _contextService;

		public CoreVideoPlayerController(ICoreVideoPlayerService service, IContextService contextService)
		{
			_service = service;
			_contextService = contextService;
		}

		[HttpGet]
		[Route("api/editor/corevideoplayer/get")]
		public IActionResult GetEditForm(Guid datasource)
		{
			var model = _service.GetData(datasource);

			return View($"~/{Settings.PathsCorePath}/Components/9_SharedComponents/CoreVideoPlayer/View/Edit.cshtml", model);
		}

		[HttpPost]
		[Route("api/editor/corevideoplayer/save")]
		public IActionResult Save([FromBody] CoreVideoPlayerModel model)
		{
			_service.SaveModel(model);

			return Ok();
		}

		[HttpGet]
		[Route("api/corevideoplayer/getvideoconfig")]
		public ActionResult GetConfig(string videoId)
		{
			if(videoId == null){
				return CreateJsonResponse(false);
			} else {
				var playerConfig = _service.GetVideo(Guid.Parse(videoId));
				
				return CreateJsonResponse(true, playerConfig);
			}
		}
	}
}
