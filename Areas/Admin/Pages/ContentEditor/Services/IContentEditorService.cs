using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using MtcMvcCore.Areas.Admin.Pages.ContentEditor.Models;
using MtcMvcCore.Core.Models;
using MtcMvcCore.Core.Models.PageModels;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Areas.Admin.Pages.ContentEditor.Services
{
	public interface IContentEditorService
	{
		ContentEditorTreeViewModel GetTreeViewModel();

		string LoadXmlDocumentContent(string filePath);
		bool SaveXmlDocumentContent(SaveFileContentModel model);
		
		bool CreateEntry(CreateEntryModel model);
		bool DeleteEntry(string path);
		bool MoveEntry(MoveEntryModel model);
		HtmlString GetPlaceholderHtml(BasePage page);
		HtmlString GetPlaceholderComponents(Guid pageId, string lang, string placeholderName);
		bool AddComponentToPlaceholder(Guid pageId, string lang, string placeholderName, string componentName);
		bool RemoveComponentFromPlaceholder(Guid pageId, string lang, string placeholderName, string componentId);
		bool SetComponentPosition(Guid pageId, string lang, string placeholderName, string componentId, int newPosition);

		bool SavePage(SaveItemDataModel model, string lang);
		List<BaseItem> GetSubItems(Guid parentId);
	}
}
