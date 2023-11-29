using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using MtcMvcCore.Core.Services;
using System;
using MtcMvcCore.Core.Models.Media;
using System.Collections.Generic;
using MtcMvcCore.Core.DataProvider;

namespace MtcMvcCore.Core.Controllers
{
	[ExcludeFromCodeCoverage]
	public class MediaDataApiController : CoreBaseController
	{
		private readonly ILogger<EditModeController> _logger;
		private readonly IMediaService _mediaService;

		private readonly IMediaDataProvider _mediaDataProvider;

		public MediaDataApiController(ILogger<EditModeController> logger, IMediaDataProvider mediaDataProvider, IMediaService mediaService)
		{
			_logger = logger;
			_mediaService = mediaService;
			_mediaDataProvider = mediaDataProvider;
		}
		

		[Route("/api/admin/media/previewinfo")]
		[HttpGet]
		[Authorize]
		public IActionResult MediaPreviewInfo(Guid mediaId)
		{
			if(mediaId == Guid.Empty) {
				return Json(new { success = false });
			}
			var mediaItem = _mediaDataProvider.GetAssetById(mediaId);

			if (mediaItem is CoreImage){
				//CoreImage img = _mediaService.GetImageById(mediaId);
				//if(img != null){
					CoreImage img = (CoreImage)mediaItem;
					var path = _mediaService.GetImagePath(img, 300);
					return Json(new { success = true, mimeType ="image", imgPath = path, img, img.Width, img.Height, img.Caption.Value, img.Caption.SimpleText });
				//} else {
				//	return Json(new { success = false});
				//}
			} else if(mediaItem is CoreVideo){
				CoreVideo video = (CoreVideo)mediaItem;
				//var path = _mediaService.GetImagePath(img, 300);
				return Json(new { success = true, mimeType ="video", video });
			} else {
				return Json(new { success = false});
			}

		}


		[Route("/api/admin/media/image/previewinfo")]
		[HttpGet]
		[Authorize]
		public IActionResult ImagePreviewInfo(Guid imgId)
		{
			if(imgId == Guid.Empty) {
				return Json(new { success = false });
			}

			CoreImage img = _mediaService.GetImageById(imgId);
			if(img != null){
				var path = _mediaService.GetImagePath(img, 300);
				return Json(new { success = true, imgPath = path, img, img.Width, img.Height, img.Caption.Value, img.Caption.SimpleText });
			} else {
				return Json(new { success = false});
			}
		}

		[Route("/api/admin/media/getassetidpath")]
		[HttpGet]
		[Authorize]
		public IActionResult GetAssetIdPath(Guid assetId)
		{
			if(assetId == Guid.Empty) {
				return Json(new { success = false });
			}

			List<Guid> parentIds = _mediaService.GetParentIds(assetId);
			parentIds.Reverse();
			return Json(new { success = true, parentIds });
		}

		[Route("/api/core/media/getdgsvideoinformations")]
		[HttpGet]
		public IActionResult GetDgsVideoInformations(Guid itemId)
		{
			if(itemId == Guid.Empty) {
				return Json(new { success = false });
			}

			var info = _mediaService.GetVideo(itemId);
			
			return CreateJsonResponse(true, new {
				basePath = info.SavePath,
				dgsFiles = info.DsgFiles
			});
		}

		[Route("/api/core/media/getresponsivimage")]
		[HttpGet]
		public IActionResult GetResponsivImage(Guid assetId)
		{
			if(assetId == Guid.Empty) {
				return CreateJsonResponse(false);
			}

			var asset = _mediaService.GetImageById(assetId);
			
			return CreateJsonResponse(true, 
			$"<picture>"+
				$"<img src='{_mediaService.GetImagePath(asset, 300)}' />" +
			"</picture>");
		}
	}
}
