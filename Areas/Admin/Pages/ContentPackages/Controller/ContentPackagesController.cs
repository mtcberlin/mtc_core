using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MtcMvcCore.Areas.Admin.Pages.ContentPackages.Services;
using MtcMvcCore.Core;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Areas.Admin.Pages.ContentPackages.Controller
{
	[Authorize(Roles = "Administrator, ContentPackages")]
	public class ContentPackagesController : AdminBaseController
	{
		private readonly IContentPackagesService _contentPackageService;

		public ContentPackagesController(IContentPackagesService contentPackageService)
		{
			_contentPackageService = contentPackageService;
		}

		[Route("admin/contentpackages")]
		public IActionResult Index()
		{
			if (Settings.AllowContentPackages)
			{
				return View($"~/{Settings.PathsCorePath}/Areas/Admin/Pages/ContentPackages/Views/Default.cshtml");
			}

			return new EmptyResult();
		}

		[Route("/api/core/contentpackages/upload")]
		[HttpPost]
		public async Task<IActionResult> Upload([FromForm] IFormFile zip)
		{
			string rootDirectory = Directory.GetCurrentDirectory();
			Directory.CreateDirectory(Path.Combine(rootDirectory, "Data", "ContentPackages"));
			var filePath = Path.Combine(rootDirectory, "Data", "ContentPackages", zip.FileName);

			try
			{
				//das kann funktionieren, kann aber auch nur zufall gewesen sein. TODO:: Beobachten
				GC.Collect();
				GC.WaitForPendingFinalizers();

				using (var stream = System.IO.File.Create(filePath))
				{
					await zip.CopyToAsync(stream);
				}

				_contentPackageService.Install(filePath);

			}
			catch (Exception e)
			{
				return Json(new { success = false, message = e.Message });
			}

			return Json(new { success = true });
		}

		[Route("/api/admin/contentpackage/serialize")]
		[HttpGet]
		public IActionResult Serialize([FromQuery] Guid pageId, [FromQuery] string name = "default", [FromQuery] bool withSubpages = true)
		{
			_contentPackageService.Create(pageId, name, withSubpages);


			return Json(new { success = true });
		}

		[Route("/api/admin/contentpackage/packages")]
		[HttpGet]
		public IActionResult Packages()
		{
			var result = _contentPackageService.GetCurrentPackages();

			return CreateJsonResponse(true, result);
		}

		[Route("/api/admin/contentpackage/delete")]
		[HttpDelete]
		public IActionResult Delete(string name)
		{
			var result = _contentPackageService.DeletePackage(name);

			if (result)
			{
				var packages = _contentPackageService.GetCurrentPackages();
				return CreateJsonResponse(true, packages);
			}

			return CreateJsonResponse(false, $"ContentPackage with name ${name} not found");
		}

		[Route("/api/admin/contentpackage/download")]
		[HttpGet]
		public IActionResult Download(string name)
		{

			var bytes = _contentPackageService.GetPackageAsByteArray(name);

			if(bytes == null) {
				return CreateJsonResponse(false, $"ContentPackage with name ${name} not found");
			}		

			return File(bytes, "application/zip", $"{name}.zip");
		}
	}

}