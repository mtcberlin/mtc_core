using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MtcMvcCore.Areas.Admin.Pages.AssetEditor.Models;
using MtcMvcCore.Areas.Admin.Pages.AssetLib.Services;
using MtcMvcCore.Areas.Admin.Pages.ContentPackages.Controller;
using MtcMvcCore.Core;
using MtcMvcCore.Core.Models;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Areas.Admin.Pages.Asset.Controller
{
	[Authorize(Roles = "Administrator, AssetLib")]
	public class AssetLibController : AdminBaseController
	{
		private readonly IAssetLibService _assetLibService;

		public AssetLibController(IAssetLibService assetLibService)
		{
			_assetLibService = assetLibService;
		}

		[Route("admin/assetlib")]
		public IActionResult Index()
		{
			if (Settings.AllowAssetLib)
			{
				var model = _assetLibService.GetTreeViewRootModel();
				return View($"~/{Settings.PathsCorePath}/Areas/Admin/Pages/AssetLib/Views/Default.cshtml", model);
			}

			return new EmptyResult();
		}

		[HttpGet]
		[Route("api/core/assetlib/getitem")]
		public IActionResult GetItem(Guid id)
		{
			var definition = _assetLibService.GetItemDefinition(id);
			//var asset = _assetLibService.GetAssetById(id);

			return CreateJsonResponse(true, new
			{
				definition,
				id = id
			});
		}

		[HttpGet]
		[Route("api/core/assetlib/getiteminfo")]
		public IActionResult GetItemInfo(Guid id)
		{
			var asset = _assetLibService.GetAssetById(id);

			return CreateJsonResponse(true, new
			{
				asset.Name
			});
		}

		[HttpPost]
		[Route("api/core/assetlib/saveitem")]
		public IActionResult SaveItem([FromBody] SaveItemDataModel model)
		{
			var result = _assetLibService.UpdateItem(model);
			return CreateJsonResponse(result);
		}

		[Route("/api/core/deleteasset")]
		[HttpGet]
		public IActionResult DeleteAsset(Guid assetId)
		{
			var result = _assetLibService.Delete(assetId);
			return Redirect("/admin/assetlib");
		}

		[HttpGet]
		[Route("api/core/assetlib/addimage")]
		public IActionResult AddImage(Guid parentId)
		{
			var success = _assetLibService.CreateImageModel(parentId);
			var subItems = _assetLibService.GetSubItems(parentId, string.Empty);
			return Json(new { success = success, subItems = subItems });
		}

		[HttpGet]
		[Route("api/core/assetlib/addfolder")]
		public IActionResult AddFolder(Guid parentId)
		{
			var success = _assetLibService.CreateFolderModel(parentId);
			var subItems = _assetLibService.GetSubItems(parentId, string.Empty);
			return Json(new { success = success, subItems = subItems });
		}

		[HttpGet]
		[Route("api/core/assetlib/addvideo")]
		public IActionResult AddVideo(Guid parentId)
		{
			var success = _assetLibService.CreateVideoModel(parentId);
			var subItems = _assetLibService.GetSubItems(parentId, string.Empty);
			return CreateJsonResponse(success, new { subItems = subItems });
		}

		[HttpGet]
		[Route("api/core/assetlib/subitems")]
		public IActionResult SubItems(Guid parentId, string type)
		{
			var subItems = _assetLibService.GetSubItems(parentId, type);
			return Json(new { success = true, subItems = subItems });
		}

		[HttpGet]
		[Route("api/core/assetlib/getassetframe")]
		public IActionResult AddFolder(Guid id, string type)
		{
			var model = _assetLibService.GetAssetById(id);
			return View($"~/{Settings.PathsCorePath}/Areas/Admin/Pages/AssetLib/Views/{type}.cshtml", model);
		}

		///TODO:: move to MediaApiController
		// TODO: RquestSizeLimit move in config
		[HttpPost]
		[RequestSizeLimit(100_000_000)]
		[Route("api/core/assetlib/upload/file")]
		public async Task<IActionResult> SetImage([FromForm] IFormFile file, [FromForm] Guid id)
		{
			return await HandleUploadFile(file, id);
		}

		///TODO:: move to MediaApiController
		private async Task<IActionResult> HandleUploadFile(IFormFile file, Guid id)
		{
			string rootPath = "wwwroot";
			string rootDirectory = Directory.GetCurrentDirectory();
			Directory.CreateDirectory(Path.Combine(rootDirectory, rootPath, "uploads", id.ToString()));
			string filePath = Path.Combine(rootDirectory, rootPath, "uploads", id.ToString(), file.FileName);

			try
			{
				//das kann funktionieren, kann aber auch nur zufall gewesen sein. TODO:: BEobachten
				GC.Collect();
				GC.WaitForPendingFinalizers();

				using (var stream = System.IO.File.Create(filePath))
				{
					await file.CopyToAsync(stream);
				}

			}
			catch (Exception e)
			{
				return Json(new { success = false, message = e.Message });
			}

			double fileSize = Math.Round((double)file.Length / 1000000, 2);
			return Json(new { success = true, imgPath = $"/uploads/{id.ToString()}/{file.FileName}", fileName = file.FileName, mimeType = file.ContentType, fileSize = fileSize });
		}

		[Route("api/core/assetlib/reorder")]
		[HttpPost]
		[Authorize]
		public IActionResult Reorder([FromBody] ReorderAssetsModel data)
		{
			foreach(var itemInfo in data.Items) {
				var asset = _assetLibService.GetAssetById(itemInfo.ItemId);
				asset.ParentId = itemInfo.ParentId;
				asset.Sort = itemInfo.Sort;
				_assetLibService.SaveAsset(asset);
			}
			
			return Ok();
		}

	}

}