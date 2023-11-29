using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using MtcMvcCore.Core.Services;
using System;
using MtcMvcCore.Core.Models.Media;
using MtcMvcCore.Core.Models;
using System.Collections.Generic;

namespace MtcMvcCore.Core.Controllers
{
	[ExcludeFromCodeCoverage]
	public class DataApiController : CoreBaseController
	{
		private readonly ILogger<EditModeController> _logger;
		private readonly IMediaService _mediaService;

		public DataApiController(ILogger<EditModeController> logger, IMediaService mediaService)
		{
			_logger = logger;
			_mediaService = mediaService;
		}

		[Route("/api/admin/data/getidpath")]
		[HttpGet]
		[Authorize(Roles = "Administrator")]
		public IActionResult GetIdPath(Guid assetId)
		{
			if(assetId == Guid.Empty) {
				return Json(new { success = false });
			}

			List<Guid> parentIds = _mediaService.GetParentIds(assetId);
			parentIds.Reverse();
			return Json(new { success = true, parentIds });
		}

	}
}
