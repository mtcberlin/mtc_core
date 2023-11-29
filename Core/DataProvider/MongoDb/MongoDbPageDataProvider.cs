﻿using System;
using System.Collections.Generic;
using System.Linq;
using MtcMvcCore.Core.Constants;
using MtcMvcCore.Core.Models;
using MtcMvcCore.Core.Models.PageModels;
using NLog;
using Microsoft.AspNetCore.Http;
using MtcMvcCore.Core.Helper;
using static MtcMvcCore.Core.Models.PageTypeConfigModel;
using System.Security.Claims;
using Microsoft.Extensions.Localization;
using MtcMvcCore.Areas.Admin.Pages.ContentEditor.Controller;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.DataProvider.MongoDb
{
	public class MongoDbPageDataProvider : IPageDataProvider
	{
		private readonly Logger _logger;
		private readonly IMongoDbDataProvider _dbDataProvider;
		private readonly IComponentDataProvider _componentDataProvider;
		private readonly IHttpContextAccessor _httpContextAccessor;

		private readonly IStringLocalizer<ContentEditorController> _stringLocalizer;

		public MongoDbPageDataProvider(IMongoDbDataProvider dbDataProvider, IHttpContextAccessor httpContextAccessor, IComponentDataProvider componentDataProvider, IStringLocalizer<ContentEditorController> stringLocalizer)
		{
			_logger = LogManager.GetCurrentClassLogger();
			_dbDataProvider = dbDataProvider;
			_componentDataProvider = componentDataProvider;
			_httpContextAccessor = httpContextAccessor;
			_stringLocalizer = stringLocalizer;
		}

		public void AddPage(Guid parentId, string lang, string pageTypeName)
		{
			var pageType = SiteConfiguration.PageTypes[pageTypeName];
			if (pageType != null)
			{
				var pageId = CreatePage(parentId, lang, pageTypeName);
				if (pageType.DefaultComponents.Any() || !string.IsNullOrEmpty(pageType.DefaultLayout) || !string.IsNullOrEmpty(pageType.DefaultView) || pageType.SubStructure.Any())
				{
					AddDefaults(pageId, lang, new BrunchPage
					{
						DefaultComponents = pageType.DefaultComponents,
						Layout = pageType.DefaultLayout,
						View = pageType.DefaultView,
						SubStructure = pageType.SubStructure
					});
				}
			}
		}

		private Guid CreatePage(Guid parentId, string lang, string pageTypeName)
		{
			var pageType = SiteConfiguration.PageTypes[pageTypeName];
			if (pageType != null)
			{
				var pageId = Guid.NewGuid();
				Type t = Type.GetType(pageType.Model);
				var inst = Activator.CreateInstance(t);
				t.GetProperty("Id").SetValue(inst, Guid.NewGuid());
				t.GetProperty("GroupId").SetValue(inst, pageId);
				t.GetProperty("Name").SetValue(inst, "New Page");
				t.GetProperty("Language").SetValue(inst, lang);
				t.GetProperty("Version").SetValue(inst, 1);
				t.GetProperty("ParentId").SetValue(inst, parentId);
				SetStatistikData(t, inst);
				_dbDataProvider.Create(inst);
				return pageId;
			}
			return Guid.Empty;
		}

		private void AddDefaults(Guid pageId, string lang, BrunchPage pageType)
		{
			var page = (BasePage)GetPage(pageId, lang);

			page.Layout = pageType.Layout;
			page.View = pageType.View;

			if (pageType.DefaultComponents != null)
			{
				page.Placeholders = new List<Placeholder>();
				var grouped = pageType.DefaultComponents.GroupBy(i => i.Placeholder);
				foreach (var place in grouped)
				{
					page.Placeholders.Add(new Placeholder
					{
						Name = place.Key,
						Components = place.Select(i => new Component
						{
							ComponentId = Guid.NewGuid().ToString(),
							Name = i.Name,
							Datasource = _componentDataProvider.CreateDatasource(i.Name).ToString()
						}).ToList()
					});
				}
			}

			SavePage(page);

			if (pageType.SubStructure != null)
			{
				foreach (var subpage in pageType.SubStructure)
				{
					var subpageType = SiteConfiguration.PageTypes[subpage.Type];
					if (subpageType != null)
					{
						var subpageId = CreatePage(pageId, lang, subpage.Type);

						AddDefaults(subpageId, lang, subpage);
					}
				}
			}
		}

		/**
		*	Returns all Pages with Language and last Content Version
		*/
		public IEnumerable<BaseItem> AllPages()
		{
			var result = new List<BaseItem>();
			var roots = _dbDataProvider.Where<BaseItem, Guid>("ParentId", Guids.WebRoot);

			var roots2 = _dbDataProvider.WhereRaw<BaseItem, Guid>("ParentId", Guids.WebRoot);
			
			var pages = roots.GroupBy(i => new { i.GroupId, i.Language }).Select(g => g.MaxBy(i => i.Version));
			AddSubPages(pages.ToList(), result);
			return result;
		}

		private void AddSubPages(List<BaseItem> pages, List<BaseItem> result)
		{
			foreach (var page in pages)
			{
				var lastVersion = pages.Where(i => i.GroupId == page.GroupId && i.Language == page.Language).MaxBy(i => i.Version);
				result.Add(lastVersion);
				var subPages = _dbDataProvider.Where<BaseItem, Guid>("ParentId", page.GroupId).Where(i => i.Language == page.Language).ToList();
				AddSubPages(subPages, result);
			}
		}

		/**
		*	Returns all Pages without Language and last Content Version
		*/
		public IEnumerable<BaseItem> PagesForTreeView()
		{
			var pages = _dbDataProvider.Where<BaseItem, Guid>("ParentId", Guids.PageRoot);

			return pages;
		}

		public void DeletePage(Guid pageId)
		{
			DeleteSubPages(pageId);
		}

		public BasePage GetPageProperties(Guid pageId, string lang)
		{
			var page = GetPage(pageId, lang);
			return (BasePage)page;
		}

		public bool SavePage(BaseItem model)
		{
			try
			{
				var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
				model.Updated = DateTime.Now;
				model.UpdatedBy = userIdClaim.Value;
				_dbDataProvider.Update(model, model.Id);
				return true;
			}
			catch
			{
				return false;
			}
		}

		private void DeleteSubPages(Guid pageId)
		{
			var versions = _dbDataProvider.Where<BasePage, Guid>("GroupId", pageId);
			var subpages = _dbDataProvider.Where<BasePage, Guid>("ParentId", pageId);

			foreach (var page in subpages)
			{
				DeleteSubPages(page.GroupId);
			}

			foreach (var version in versions)
			{
				_dbDataProvider.Delete<BasePage>(version.Id);
			}
		}

		public BaseItem GetPage(Guid pageId, string lang)
		{
			var versions = _dbDataProvider.Where<BaseItem, Guid>("GroupId", pageId).Where(i => i.Language == lang);
			var page = versions.MaxBy(i => i.Version);
			return page;
		}

		public List<BasePage> GetPages(Guid pageId, bool withSubpages)
		{
			var pages = _dbDataProvider.Where<BasePage, Guid>("GroupId", pageId);

			if (withSubpages)
			{
				AddSubPages(pages, pageId);
			}
			return pages;
		}

		private void AddSubPages(List<BasePage> result, Guid parentId)
		{
			var subPages = _dbDataProvider.Where<BasePage, Guid>("ParentId", parentId);
			result.AddRange(subPages);
			foreach (var subPage in subPages.DistinctBy(i => i.GroupId))
			{
				AddSubPages(result, subPage.GroupId);
			}
		}

		public bool SavePages(object pageData, bool checkParent)
		{
			var data = (BasePage)pageData;
			_logger.Info($"Install Page Data {data.Id}");
			var oldData = _dbDataProvider.Get<BasePage, Guid>("Id", data.Id);

			if (checkParent)
			{
				var parent = _dbDataProvider.Get<BasePage, Guid>("Id", data.ParentId);
				if (parent == null)
				{
					data.ParentId = Guids.PageRoot;
				}
			}

			if (oldData.Id != Guid.Empty)
			{
				return _dbDataProvider.Update(data, data.Id);
			}
			else
			{
				_dbDataProvider.Create(data);
				return true;
			}
		}

		public List<EditorFieldDefinition> GetPageFieldDefinitions(Guid pageId, string type, string lang)
		{
			List<EditorFieldDefinition> fields = new List<EditorFieldDefinition>();

			var page = GetPage(pageId, lang);
			if (page != null)
			{
				ModelToEditorFieldDefinition.CreateEditorFieldDefinitions(page.GetType(), fields, "", page, _httpContextAccessor.HttpContext.User, _stringLocalizer);
			}

			return fields;
		}

		public List<BaseItem> GetSubpages(Guid parentId)
		{
			var q = _dbDataProvider.Where<BaseItem, Guid>("ParentId", parentId);
			var pages = q.GroupBy(i => new { i.GroupId, i.Language }).Select(g => g.MaxBy(i => i.Version));
			foreach (var page in pages)
			{
				page.SubItems = GetSubpages(page.GroupId);
				page.HasSubItems = page.SubItems.Any();
			}
			return pages.ToList();
		}

		public string[] GetLanguages(Guid pageId)
		{
			var pages = _dbDataProvider.Where<BaseItem, Guid>("GroupId", pageId);
			return pages.Select(i => i.Language).Distinct().ToArray();
		}

		public BasePage AddLanguage(Guid pageId, string lang)
		{
			var pages = GetPages(pageId, false);
			if (!pages.Any(i => i.Language == lang))
			{
				var page = pages.First();
				var pageType = SiteConfiguration.PageTypes[page.GetType().Name];
				if (pageType != null)
				{
					Type t = Type.GetType(pageType.Model);
					var inst = Activator.CreateInstance(t);
					t.GetProperty("Id").SetValue(inst, Guid.NewGuid());
					t.GetProperty("GroupId").SetValue(inst, pageId);
					t.GetProperty("Name").SetValue(inst, page.Name);
					t.GetProperty("Language").SetValue(inst, lang);
					t.GetProperty("Version").SetValue(inst, 1);
					t.GetProperty("ParentId").SetValue(inst, page.ParentId);
					SetStatistikData(t, inst);
					_dbDataProvider.Create(inst);
					return (BasePage)inst;
				}
			}
			return null;
		}

		private void SetStatistikData(Type t, object inst)
		{
			var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
			t.GetProperty("Created").SetValue(inst, DateTime.Now);
			t.GetProperty("Updated").SetValue(inst, DateTime.Now);
			t.GetProperty("CreatedBy").SetValue(inst, userIdClaim.Value);
			t.GetProperty("UpdatedBy").SetValue(inst, userIdClaim.Value);
		}
	}
}
