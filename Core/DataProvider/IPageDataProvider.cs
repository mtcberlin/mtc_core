using System;
using System.Collections.Generic;
using MtcMvcCore.Core.Models;
using MtcMvcCore.Core.Models.PageModels;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.DataProvider
{
	public interface IPageDataProvider
	{
		IEnumerable<BaseItem> AllPages();
		IEnumerable<BaseItem> PagesForTreeView();
		void AddPage(Guid parentId, string lang, string pageType);
		void DeletePage(Guid pageId);
		bool SavePage(BaseItem model);
		BasePage GetPageProperties(Guid pageId, string lang);
		List<EditorFieldDefinition> GetPageFieldDefinitions(Guid pageId, string type, string lang);
		BaseItem GetPage(Guid pageId, string lang);
		BasePage AddLanguage(Guid pageId, string lang);
		string[] GetLanguages(Guid pageId);
		
		/// <summary>
		/// returns just the page object without lang, version and pageconfig
		/// used to serialize for content package
		/// </summary>
		/// <param></param>
		/// <returns>Page</returns>
		List<BasePage> GetPages(Guid pageId, bool withSubpages);
		
		bool SavePages(object pageData, bool checkParent);
		
		List<BaseItem> GetSubpages(Guid parentId);
	}
}
