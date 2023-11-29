using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using MtcMvcCore.Areas.Admin.Pages.ContentEditor.Models;
using MtcMvcCore.Areas.Admin.Pages.ContentEditor.Services;
using MtcMvcCore.Areas.Admin.Pages.ContentPackages.Controller;
using MtcMvcCore.Core;
using MtcMvcCore.Core.Caching;
using MtcMvcCore.Core.DataProvider;
using MtcMvcCore.Core.Models;
using MtcMvcCore.Core.Models.Exceptions;
using MtcMvcCore.Core.Models.PageModels;
using MtcMvcCore.Core.Services;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Areas.Admin.Pages.ContentEditor.Controller
{
	public class ContentEditorController : AdminBaseController
	{
		private readonly IContentEditorService _contentEditorService;
		private readonly IPageDataProvider _pageDataProvider;

		private readonly IUrlService _urlService;

		public ContentEditorController(IContentEditorService contentEditorService, IPageDataProvider pageDataProvider, IUrlService urlService)
		{
			_contentEditorService = contentEditorService;
			_pageDataProvider = pageDataProvider;
			_urlService = urlService;
		}

		[Route("admin/contenteditor")]
		[Authorize(Roles = "Administrator, SuperEditor, ContentEditor")]
		public IActionResult Index()
		{
			if (Settings.AllowContentEditor)
			{
				var model = _contentEditorService.GetTreeViewModel();
				return View(
					$"~/{Settings.PathsCorePath}/Areas/Admin/Pages/ContentEditor/Views/Contenteditor.cshtml",
					model);
			}

			return new EmptyResult();
		}

		[Route("/admin/getfilecontent")]
		[HttpGet]
		public IActionResult GetFileContent([FromQuery] string path)
		{
			var doc = _contentEditorService.LoadXmlDocumentContent(path);
			return Content(doc);
		}

		[Route("/admin/savefilecontent")]
		[HttpPost]
		public IActionResult SaveFileContent([FromBody] SaveFileContentModel model)
		{
			if (_contentEditorService.SaveXmlDocumentContent(model))
			{
				SiteConfiguration.ReadConfigurations();
				CacheManager.Clear();
				return Json(new { RequireRefresh = !model.Filepath.Contains(".edited.xml") });
			}

			return BadRequest();
		}

		[Route("/admin/createentry")]
		[HttpPost]
		[Authorize(Roles = "Administrator, SuperEditor, ContentEditor")]
		public IActionResult CreateEntry([FromBody] CreateEntryModel model)
		{
			if (_contentEditorService.CreateEntry(model))
			{
				return Json(new { RequireRefresh = true });
			}

			return BadRequest();
		}

		[Route("/admin/moveentry")]
		[HttpPost]
		[Authorize(Roles = "Administrator, SuperEditor, ContentEditor")]
		public IActionResult MoveEntry([FromBody] MoveEntryModel model)
		{
			if (_contentEditorService.MoveEntry(model))
			{
				return Json(new { RequireRefresh = true });
			}

			return BadRequest();
		}

		[Route("/admin/deleteentry")]
		[HttpPost]
		[Authorize(Roles = "Administrator, SuperEditor, ContentEditor")]
		public IActionResult DeleteEntry([FromBody] DeleteEntryModel model)
		{
			if (_contentEditorService.DeleteEntry(model.Path))
			{
				return Ok();
			}

			return BadRequest();
		}

		[Route("/admin/contenteditor/page/getItemProperties")]
		[HttpGet]
		[Authorize(Roles = "Administrator, SuperEditor, ContentEditor")]
		public IActionResult GetPageProperties(Guid id, string lang, string type)
		{
			if (string.IsNullOrEmpty(lang))
			{
				lang = Settings.DefaultLanguage;
			}

			var pageFields = _pageDataProvider.GetPageFieldDefinitions(id, type, lang);
			var props = _pageDataProvider.GetPage(id, lang);
			var languages = _pageDataProvider.GetLanguages(id);
			if (pageFields.Count == 0)
			{
				return CreateJsonResponse(false, new
				{
					Id = id,
					Language = lang,
					Languages = languages
				});
			}

			HtmlString placeholderHtml = null;

			if (type == "page")
			{
				placeholderHtml = _contentEditorService.GetPlaceholderHtml((BasePage)props);
			}

			return CreateJsonResponse(true, new
			{
				Definition = pageFields,
				Languages = languages,
				PlaceholderHtml = placeholderHtml,
				Id = props.GroupId,
				Url = _urlService.GetLinkInformationById(id, lang).Href,
				Language = lang
			});
		}

		[Route("/admin/getPlaceholderHtml")]
		[HttpGet]
		[Authorize(Roles = "Administrator, SuperEditor, ContentEditor")]
		public IActionResult GetPlaceholderHtml(Guid pageId, string lang)
		{
			var page = _pageDataProvider.GetPage(pageId, lang);
			if (page.GetType() != typeof(BasePage) && !page.GetType().IsSubclassOf(typeof(BasePage)))
			{
				return Json(new
				{
					Data = new
					{
						PlaceholderHtml = string.Empty
					}
				});
			}
			var placeholderHtml = _contentEditorService.GetPlaceholderHtml((BasePage)page);
			return Json(new
			{
				Data = new
				{
					PlaceholderHtml = placeholderHtml
				}
			});
		}

		[Route("/api/core/contenteditor/addpage")]
		[HttpGet]
		[Authorize(Roles = "Administrator, SuperEditor, ContentEditor")]
		public IActionResult AddPage(Guid parentId, string lang, string type = "Page")
		{
			try
			{
				_pageDataProvider.AddPage(parentId, lang, type);
				SiteConfiguration.ReadConfigurations();
				CacheManager.Clear();
				return Json(new { Success = true });
			}
			catch (InvalidPageTypeException e)
			{
				
			}
			return BadRequest(new { Success = false });
		}

		[Route("/admin/contenteditor/deletepage")]
		[HttpGet]
		[Authorize(Roles = "Administrator, SuperEditor, ContentEditor")]
		public IActionResult DeletePage(Guid pageId)
		{
			_pageDataProvider.DeletePage(pageId);
			SiteConfiguration.ReadConfigurations();
			CacheManager.Clear();
			return Redirect("/admin/contenteditor");
		}

		[HttpPost]
		[Route("/admin/contenteditor/page/saveItemProperties")]
		[Authorize(Roles = "Administrator, SuperEditor, ContentEditor")]
		public IActionResult SavePage([FromBody] SaveItemDataModel model)
		{
			if(model != null){
				if (_contentEditorService.SavePage(model, model.lang))
				{
					SiteConfiguration.ReadConfigurations();
					CacheManager.Clear();
					return CreateJsonResponse(true, new { RequireRefresh = false });
				}
			}
			return BadRequest();
		}

		[Route("/api/core/contenteditor/addVersion")]
		[HttpGet]
		[Authorize(Roles = "Administrator, SuperEditor, ContentEditor")]
		public IActionResult AddVersion(Guid pageId, string type, string lang)
		{
			if (!Settings.AllowedLanguage.Contains(lang))
			{
				return CreateJsonResponse(false, default(object), "The given Language is not allowed to add. Please contact a admistrator.");
			}

			var newPage = _pageDataProvider.AddLanguage(pageId, lang);

			var pageFields = _pageDataProvider.GetPageFieldDefinitions(pageId, type, lang);
			var languages = _pageDataProvider.GetLanguages(pageId);
			var placeholderHtml = _contentEditorService.GetPlaceholderHtml(newPage);
			return Json(new
			{
				Data = new
				{
					Definition = pageFields,
					PlaceholderHtml = placeholderHtml,
					Id = newPage.GroupId,
					Language = lang,
					Languages = languages
				}
			});
		}

		[Route("/admin/contenteditor/getPlaceholderComponents")]
		[HttpGet]
		[Authorize(Roles = "Administrator, SuperEditor, ContentEditor")]
		public IActionResult GetPlaceholderComponents(Guid pageId, string lang, string placeholderName)
		{
			var placeholderHtml = _contentEditorService.GetPlaceholderComponents(pageId, lang, placeholderName);
			return Json(new { Data = placeholderHtml });
		}

		[Route("/admin/contenteditor/addComponent")]
		[HttpPost]
		[Authorize(Roles = "Administrator, SuperEditor, ContentEditor")]
		public IActionResult AddComponent([FromBody] ChangeComponentModel data)
		{
			var props = _contentEditorService.AddComponentToPlaceholder(data.PageId, data.Lang, data.PlaceholderName, data.ComponentName);
			SiteConfiguration.ReadConfigurations();
			CacheManager.Clear();
			return Json(props);
		}

		[Route("/admin/contenteditor/removeComponent")]
		[HttpPost]
		[Authorize(Roles = "Administrator, SuperEditor, ContentEditor")]
		public IActionResult RemoveComponent([FromBody] ChangeComponentModel data)
		{
			var props = _contentEditorService.RemoveComponentFromPlaceholder(data.PageId, data.Lang, data.PlaceholderName, data.ComponentId);
			SiteConfiguration.ReadConfigurations();
			CacheManager.Clear();
			return Json(props);
		}

		[Route("/admin/contenteditor/setComponentPosition")]
		[HttpPost]
		[Authorize(Roles = "Administrator, SuperEditor, ContentEditor")]
		public IActionResult SetComponentPosition([FromBody] ChangeComponentModel data)
		{
			var props = _contentEditorService.SetComponentPosition(data.PageId, data.Lang, data.PlaceholderName, data.ComponentId, int.Parse(data.NewPosition));
			SiteConfiguration.ReadConfigurations();
			CacheManager.Clear();
			return Json(props);
		}

		[HttpGet]
		[Route("api/core/contenteditor/gettreesubitems")]
		[Authorize(Roles = "Administrator, SuperEditor, ContentEditor")]
		public IActionResult GetTreeSubItems(Guid parentId)
		{
			var subItems = _contentEditorService.GetSubItems(parentId);

			var result = subItems.Select(i => new
			{
				Id = i.GroupId,
				i.InsertOptions,
				i.Name,
				i.ParentId,
				type = i is BasePage ? "page" : i.Type,
				i.HasSubItems
			});

			return Json(new
			{
				success = true,
				subItems = result
			});
		}

		[HttpGet]
		[Route("api/core/contenteditor/getiteminfo")]
		[Authorize(Roles = "Administrator, SuperEditor, ContentEditor")]
		public IActionResult GetItemInfo(Guid id)
		{
			var page = _pageDataProvider.GetPage(id, Settings.DefaultLanguage);
			return CreateJsonResponse(true, new
			{
				page.Name
			});
		}

		[Route("api/core/contenteditor/reorder")]
		[HttpPost]
		[Authorize]
		public IActionResult Reorder([FromBody] ReorderPagesModel data)
		{
			foreach(var pageInfo in data.Pages) {
				var pageGroup = _pageDataProvider.GetPages(pageInfo.PageId, false);
				foreach (var page in pageGroup)
				{
					page.ParentId = pageInfo.ParentId;
					page.Sort = pageInfo.Sort;
					_pageDataProvider.SavePage(page);
				}
			}
			SiteConfiguration.ReadConfigurations();
			CacheManager.Clear();
			return Ok();
		}

	}

}