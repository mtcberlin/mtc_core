using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Logging;
using MtcMvcCore.Areas.Admin.Pages.ContentEditor.Models;
using MtcMvcCore.Core;
using MtcMvcCore.Core.DataProvider;
using MtcMvcCore.Core.Helper;
using MtcMvcCore.Core.Models;
using MtcMvcCore.Core.Models.PageModels;
using MtcMvcCore.Core.Services;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Areas.Admin.Pages.ContentEditor.Services
{
	public class ContentEditorService : IContentEditorService
	{
		private static string[] complexExcludedTypes = new[] { "decimal", "string", "datetime", "guid" };
		private readonly IContextService _contextService;
		private readonly IPageDataProvider _pageDataProvider;
		private readonly IComponentDataProvider _componentDataProvider;
		private readonly ILogger<ContentEditorService> _logger;

		public ContentEditorService(ILogger<ContentEditorService> logger, IContextService contextService, IPageDataProvider pageDataProvider, IComponentDataProvider componentDataProvider)
		{
			_logger = logger;
			_contextService = contextService;
			_pageDataProvider = pageDataProvider;
			_componentDataProvider = componentDataProvider;
		}


		public ContentEditorTreeViewModel GetTreeViewModel()
		{
			var pages = _pageDataProvider.PagesForTreeView();
			var roots = pages.Where(i => i.ParentId == Guid.Parse("{11111111-1111-1111-1111-111111111111}")).DistinctBy(i => i.GroupId);

			foreach (var root in roots)
			{
				root.HasSubItems = _pageDataProvider.GetSubpages(root.GroupId).Any();
				root.Type = root.GetType().Name;
				root.InsertOptions = GetInsertOptions(root.Type);
			}

			return new ContentEditorTreeViewModel
			{
				Roots = roots.ToList()
			};
		}

		private static void SetPageChildren(BaseItem page, IEnumerable<BaseItem> pages)
		{
			var children = pages.Where(i => i.ParentId == page.Id).ToList();

			foreach (var child in children)
			{
				SetPageChildren(child, pages);
			}

			page.SubItems = children;
		}

		public string LoadXmlDocumentContent(string filePath)
		{
			try
			{
				return System.IO.File.ReadAllText(WebUtility.UrlDecode(filePath));
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"Error while loading {filePath}");
			}

			return null;
		}

		private BaseItem FindPage(List<BaseItem> pages, string pageName)
		{
			BaseItem result = null;
			foreach (var page in pages)
			{
				if (page.Name == pageName)
				{
					result = page;
					break;
				}

				if (page.SubItems.Count > 0)
				{
					var found = FindPage(page.SubItems, pageName);
					if (found != null)
					{
						result = found;
					}
				}
			}

			return result;
		}

		public bool SaveXmlDocumentContent(SaveFileContentModel model)
		{
			try
			{
				var orginalFilePath = WebUtility.UrlDecode(model.Filepath);
				if (string.IsNullOrEmpty(orginalFilePath)) return false;

				var m = new XmlDocument();
				if (orginalFilePath.Contains(".edited.xml") || !Settings.EnableFileVersions)
				{
					m.Load(orginalFilePath);
					m.InnerXml = model.Content;
					m.Save(orginalFilePath);
				}
				else
				{
					m.LoadXml(model.Content);
					m.Save(orginalFilePath.Replace(".xml", ".edited.xml"));
				}

				return true;
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"Error while loading {model.Filepath}");
			}

			return false;
		}

		public bool CreateEntry(CreateEntryModel model)
		{
			try
			{
				var parentPath = WebUtility.UrlDecode(model.Path);
				if (string.IsNullOrEmpty(parentPath)) return false;

				if (model.Type == "folder")
				{
					Directory.CreateDirectory(Path.Combine(parentPath, model.Name));
				}
				else if (model.Type == "file")
				{
					var m = new XmlDocument();
					m.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\n<root></root>");
					if (Settings.EnableFileVersions)
					{
						m.Save($"{parentPath}/{model.Name.Replace(".xml", string.Empty)}.edited.xml");
					}
					else
					{
						m.Save($"{parentPath}/{model.Name}");
					}
				}

				return true;
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"Error while creating entry {model.Path} {model.Name} {model.Type}");
			}

			return false;
		}

		public bool MoveEntry(MoveEntryModel model)
		{
			try
			{
				var oldPath = WebUtility.UrlDecode(model.OldPath);
				var newPath = WebUtility.UrlDecode(model.NewPath);
				if (string.IsNullOrEmpty(oldPath)) return false;
				if (string.IsNullOrEmpty(newPath)) return false;

				if (oldPath.EndsWith(".xml"))
				{
					File.Move(oldPath, newPath);
				}
				else
				{
					Directory.Move(oldPath, newPath);
				}

				return true;
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"Error while moving entry {model.OldPath} to {model.NewPath}");
			}

			return false;
		}

		public bool DeleteEntry(string path)
		{
			try
			{
				var deletePath = WebUtility.UrlDecode(path);
				if (string.IsNullOrEmpty(deletePath)) return false;

				if (deletePath.EndsWith(".xml"))
				{
					File.Delete(deletePath);
				}
				else
				{
					Directory.Delete(deletePath, true);
				}

				return true;
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"Error while deleting entry {path}");
			}

			return false;
		}

		private ContentEditorFolderInfo GetInfosForFolder(string path)
		{
			var contentInfo = new ContentEditorFolderInfo();
			contentInfo.Name = "Data";
			contentInfo.Path = "Data";
			var files = Directory.GetFiles(path, "*.xml", SearchOption.TopDirectoryOnly);
			foreach (var file in files)
			{
				if (file.EndsWith("edited.xml"))
				{
					AddFileToContentInfo(contentInfo, file);
				}
				else
				{
					var editedPath = file.Replace(".xml", ".edited.xml");
					if (!files.Contains(editedPath))
					{
						AddFileToContentInfo(contentInfo, file);
					}
				}
			}

			var folders = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);

			foreach (var folder in folders)
			{
				var folderInfo = GetInfosForFolder(folder);
				folderInfo.Name = new DirectoryInfo(folder).Name;
				folderInfo.Path = WebUtility.UrlEncode(folder);
				contentInfo.Folder.Add(folderInfo);
			}

			return contentInfo;
		}

		private void AddFileToContentInfo(ContentEditorFolderInfo collection, string path)
		{
			var fileInfo = new FileInfo(path);
			collection.Files.Add(new ContentEditorFileInfo
			{
				Name = fileInfo.Name,
				Path = WebUtility.UrlEncode(path)
			});
		}

		// public HtmlString GetPlaceholderHtml(PageConfigurationModel pageConfiguration)
		// {
		// 	var result = new StringBuilder();
		// 	result.Append($"<core-accordion data-allow-multiple>");
		// 	List<string> placeholderNames = new List<string>();
		// 	if (!string.IsNullOrEmpty(pageConfiguration.Layout.Contentview) && SiteConfiguration.ContentLayouts.ContainsKey(pageConfiguration.Layout.Contentview))
		// 	{
		// 		var contentView = SiteConfiguration.ContentLayouts[pageConfiguration.Layout.Contentview];
		// 		placeholderNames.AddRange(contentView.Placeholders.Select(i => i.Name));
		// 	}

		// 	if (!string.IsNullOrEmpty(pageConfiguration.Layout.Pagelayout) && SiteConfiguration.Layouts.ContainsKey(pageConfiguration.Layout.Pagelayout))
		// 	{
		// 		var pageLayout = SiteConfiguration.Layouts[pageConfiguration.Layout.Pagelayout];
		// 		placeholderNames.AddRange(pageLayout.Placeholders.Select(i => i.Name));
		// 	}

		// 	IEnumerable<string> extraPlace = new List<string>();
		// 	IEnumerable<string> union = placeholderNames;
		// 	if (pageConfiguration.Placeholders != null)
		// 	{
		// 		extraPlace = pageConfiguration.Placeholders.Select(i => i.Name).Except(placeholderNames);
		// 		union = placeholderNames.Union(pageConfiguration.Placeholders.Select(i => i.Name));
		// 	}

		// 	foreach (var placeholderName in union)
		// 	{
		// 		var placeholder = pageConfiguration.Placeholders?.FirstOrDefault(i => i.Name == placeholderName);
		// 		if (placeholder == null)
		// 		{
		// 			placeholder = new Placeholder { Name = placeholderName };
		// 		}
		// 		result.Append($"<core-panel expanded='false' id='{placeholderName}'><span slot='title' {(extraPlace.Contains(placeholderName) ? "style=background-color:red;" : string.Empty)}>" +
		// 		$" {placeholderName}</span><div slot='content'><editor-placeholder data-page-config-id='{pageConfiguration.Id}' data-placeholder-name='{placeholderName}'><div class='action-bar row'>{(extraPlace.Contains(placeholderName) ? string.Empty : GetAddComponentDialog(placeholderName, pageConfiguration.Id))}</div>");
		// 		result.Append(GeneratePlaceholderHtml(placeholder));
		// 		result.Append("</editor-placeholder></div></core-panel>");
		// 	}
		// 	result.Append($"</core-accordion>");
		// 	return new HtmlString(result.ToString());
		// }

		public HtmlString GetPlaceholderHtml(BasePage page)
		{
			if (page == null)
			{
				return new HtmlString(string.Empty);
			}

			LayoutConfigurationModel contentView = null;
			var result = new StringBuilder();
			//result.Append($"<core-accordion data-allow-multiple>");
			List<string> placeholderNames = new List<string>();
			if (!string.IsNullOrEmpty(page.View) && SiteConfiguration.ContentLayouts.ContainsKey(page.View))
			{
				contentView = SiteConfiguration.ContentLayouts[page.View];
				placeholderNames.AddRange(contentView.Placeholders.Select(i => i.Name));
			}

			if (!string.IsNullOrEmpty(page.Layout) && SiteConfiguration.Layouts.ContainsKey(page.Layout))
			{
				var pageLayout = SiteConfiguration.Layouts[page.Layout];
				placeholderNames.AddRange(pageLayout.Placeholders.Select(i => i.Name));
			}

			IEnumerable<string> extraPlace = new List<string>();
			IEnumerable<string> union = placeholderNames;
			if (page.Placeholders != null)
			{
				extraPlace = page.Placeholders.Select(i => i.Name).Except(placeholderNames);
				union = placeholderNames.Union(page.Placeholders.Select(i => i.Name)).Except(extraPlace);
			}
			result.Append($"<core-panel expanded='true' title='{Translation.Translate("Placeholder")}'>" +
							$"<div slot='content'><div class='overflow-auto' style='min-width: 650px'><div class='d-flex'>");
			foreach (var placeholderName in union)
			{
				var placeholderColumns = 12;
				var placeholder = page.Placeholders?.FirstOrDefault(i => i.Name == placeholderName);
				if (placeholder == null)
				{
					placeholder = new Placeholder { Name = placeholderName };
				}

				if (contentView != null)
				{
					var placeholderDefinition = contentView.Placeholders.FirstOrDefault(i => i.Name == placeholderName);
					if (placeholderDefinition != null && placeholderDefinition.Columns != null && placeholderDefinition.Columns.Any()) placeholderColumns = placeholderDefinition.Columns.Last().Columns;
				}

				result.Append($"<div class='col-{placeholderColumns}'>");
				result.Append($"<editor-placeholder data-placeholder-name='{placeholderName}'>");
				result.Append(GeneratePlaceholderHtml(placeholder, extraPlace.Contains(placeholderName), page.Id));
				result.Append("</editor-placeholder>");
				result.Append($"</div>");
			}
			result.Append($"</div></div></div></core-panel>");
			if (extraPlace.Any())
			{
				AppendOldPlaceholderContent(extraPlace, result, page);
			}
			//result.Append($"</core-accordion>");
			return new HtmlString(result.ToString());
		}

		private void AppendOldPlaceholderContent(IEnumerable<string> extraPlace, StringBuilder result, BasePage page)
		{
			result.Append($"<core-panel expanded='true' title='{Translation.Translate("Unused Components")}'>" +
							$"<div slot='content'><div class='container-fluid'><div class='row'>");
			foreach (var placeholderName in extraPlace)
			{
				var placeholder = page.Placeholders?.FirstOrDefault(i => i.Name == placeholderName);
				if (placeholder == null)
				{
					placeholder = new Placeholder { Name = placeholderName };
				}

				result.Append($"<div class='col-12'>");
				result.Append($"<editor-placeholder data-placeholder-name='{placeholderName}'>");
				result.Append(GeneratePlaceholderHtml(placeholder, true, page.Id));
				result.Append("</editor-placeholder>");
				result.Append($"</div>");
			}
			result.Append($"</div></div></div></core-panel>");
		}

		private IHtmlContent GeneratePlaceholderHtml(Placeholder placeholder, bool isExtraPlaceholder, Guid pageConfigurationId)
		{

			var s = new StringBuilder();
			s.Append($"<div data-placeholder-name='{placeholder.Name}'>");
			var i = 0;
			foreach (var component in placeholder.Components)
			{
				// s.Append($"<core-panel expanded='false'><span slot='title'>" +
				// $"{component.Name}</span><div slot='content'><div class='action-bar row'><button type='button' class='btn bi bi-trash js-remove-component' data-componentId='{component.ComponentId}'></button>{(i < placeholder.Components.Count - 1 ? $"<button type='button' class='btn bi bi-arrow-down js-move-component' data-componentId='{component.ComponentId}' data-component-position='{i + 1}'></button>" : string.Empty)}{(i == 0 ? string.Empty : $"<button type='button' class='btn bi bi-arrow-up js-move-component' data-componentId='{component.ComponentId}' data-component-position='{i - 1}'></button>")}<button type='button' class='btn bi bi-save js-save-component' data-datasource-id='{component.Datasource}'></button></div><div class='' data-datasource-id='{component.Datasource}' data-component-name='{component.Name}'>&nbsp;</div>");
				var componentConfig = SiteConfiguration.GetComponentConfig(component.Name);
				if (componentConfig != null)
				{
					s.Append($"<div><editor-edit-component data-modal-title='{Translation.Translate("Edit Component")}' data-component-name='{component.Name}' data-datasource-id='{component.Datasource}'><div class='editor-ph-component d-flex justify-content-between mb-3 me-3 border border-2 rounded border-core-gray'><div class='js-open-modal d-flex flex-column w-100'><p class='fw-bold text-break px-2 py-1 bg-light border-bottom border-1' style='hypens: auto; border-top-left-radius: 0.375rem;'>{Translation.Translate(componentConfig.DisplayName)}</p><div class='text-break px-2 py-1' style='hyphens: auto;'>{Translation.Translate(componentConfig.Description)}</div></div><div class='d-flex flex-column bg-light border-start border-2 rounded-end'>{GetComponentActions(placeholder, i)}</div></div>");
					if (componentConfig == null || string.IsNullOrEmpty(componentConfig.EditorModel))
					{
						s.Append($"No ComponentConfig found for {component.Name}");
					}
					s.Append("</editor-edit-component></div>");
					i++;
				}
			}
			if (!isExtraPlaceholder)
			{
				s.Append(GetAddComponentDialog(placeholder.Name, pageConfigurationId));
			}
			s.Append("</div>");
			return new HtmlString(s.ToString());
		}

		private string GetComponentActions(Placeholder placeholder, int i)
		{
			return $"<button type='button' class='btn bi bi-trash js-remove-component' data-componentId='{placeholder.Components[i].ComponentId}'></button>{(i < placeholder.Components.Count - 1 ? $"<button type='button' class='btn bi bi-arrow-down js-move-component' data-componentId='{placeholder.Components[i].ComponentId}' data-component-position='{i + 1}'></button>" : string.Empty)}{(i == 0 ? string.Empty : $"<button type='button' class='btn bi bi-arrow-up js-move-component' data-componentId='{placeholder.Components[i].ComponentId}' data-component-position='{i - 1}'></button>")}";
		}

		private string GetAddComponentDialog(string placeholderName, Guid pageConfigurationId)
		{
			return $"<editor-add-component buttonname='{Translation.Translate("Add Component")}' modaltitle='{Translation.Translate("Add Component")}' data-placeholder-name='{placeholderName}'></editor-add-component>";
		}

		public bool SavePage(BasePage model, string lang)
		{
			var page = (BasePage)_pageDataProvider.GetPage(model.GroupId, lang);
			TransferData(page, model);
			_pageDataProvider.SavePage(page);
			return true;
		}

		private void TransferData<T>(T target, T source)
		{
			if (source == null || target == null) return;
			PropertyInfo[] propInfos = target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (var prop in propInfos)
			{
				var editorAttribute = prop.GetCustomAttributes<EditorConfigAttribute>().FirstOrDefault();
				if (editorAttribute != null && !prop.PropertyType.IsPrimitive && !complexExcludedTypes.Contains(prop.PropertyType.Name.ToLower()))
				{
					TransferData(prop.GetValue(target, null), prop.GetValue(source, null));
				}
				else if (editorAttribute != null)
				{
					prop.SetValue(target, prop.GetValue(source, null));
				}
			}
		}

		public HtmlString GetPlaceholderComponents(Guid pageId, string lang, string placeholderName)
		{
			var page = (BasePage)_pageDataProvider.GetPage(pageId, lang);
			if (!string.IsNullOrEmpty(page.View) && SiteConfiguration.ContentLayouts.ContainsKey(page.View))
			{
				var contentLayout = SiteConfiguration.ContentLayouts[page.View];
				var placeholder = contentLayout.Placeholders.FirstOrDefault(i => i.Name == placeholderName);
				if (placeholder != null)
				{
					return GetComponentList(placeholder.AllowedComponents);
				}
			}

			if (!string.IsNullOrEmpty(page.Layout) && SiteConfiguration.Layouts.ContainsKey(page.Layout))
			{
				var pageLayout = SiteConfiguration.Layouts[page.Layout];
				var placeholder = pageLayout.Placeholders.FirstOrDefault(i => i.Name == placeholderName);
				if (placeholder != null)
				{
					return GetComponentList(placeholder.AllowedComponents);
				}
			}
			return new HtmlString("");
		}

		public HtmlString GetComponentList(List<string> allowedComponents)
		{
			var s = new StringBuilder();
			foreach (var component in allowedComponents)
			{
				var componentConfig = SiteConfiguration.GetComponentConfig(component);
				if (componentConfig != null)
				{
					//s.Append($"<li><div>Bild</div><div>{componentConfig.DisplayName}</div><div>{componentConfig.Description}</div><button type='button' data-component='{component}' class='js-add-component'>Add</button></li>");
					s.Append($"<li><div><input type='radio' id='{component}' name='component' value='{component}'><label class='w-100' for='{component}'>"
					+ $"<div class='card mb-3' style='max-width: 540px;'><div class='row g-0'><div class='col-md-4 d-flex align-items-center justify-content-center'>{GetImgTagOrDefault(componentConfig.PreviewImg, componentConfig.DisplayName)}"
					+ $"</div><div class='col-md-8'><div class='card-body'><h5 class='card-title'>{Translation.Translate(componentConfig.DisplayName)}</h5>"
					+ $"<p class='card-text'>{Translation.Translate(componentConfig.Description)}</p>"
					+ "</div></div></div></div>"
					+ $"</label><div></li>");
				}
			}
			return new HtmlString(s.ToString());
		}

		private string GetImgTagOrDefault(string imgPath, string altText)
		{
			var result = new StringBuilder();
			if (string.IsNullOrEmpty(imgPath))
			{
				result.Append("<svg xmlns='http://www.w3.org/2000/svg' height='4em' viewBox='0 0 512 512'><path d='M149.1 64.8L138.7 96H64C28.7 96 0 124.7 0 160V416c0 35.3 28.7 64 64 64H448c35.3 0 64-28.7 64-64V160c0-35.3-28.7-64-64-64H373.3L362.9 64.8C356.4 45.2 338.1 32 317.4 32H194.6c-20.7 0-39 13.2-45.5 32.8zM256 192a96 96 0 1 1 0 192 96 96 0 1 1 0-192z'/></svg>");
			}
			else
			{
				result.Append($"<img src='{imgPath}' class='img-fluid rounded-start' alt='{altText}'>");
			}


			return result.ToString();
		}

		public bool AddComponentToPlaceholder(Guid pageId, string lang, string placeholderName, string componentName)
		{
			var page = (BasePage)_pageDataProvider.GetPage(pageId, lang);
			var contentLayout = SiteConfiguration.ContentLayouts[page.View];
			bool canAdd = false;
			if (contentLayout.Placeholders.Any(i => i.Name == placeholderName))
			{
				var placeholder = contentLayout.Placeholders.First(i => i.Name == placeholderName);
				canAdd = placeholder.AllowedComponents.Any(i => i == componentName);
			}

			var pageLayout = SiteConfiguration.Layouts[page.Layout];
			if (pageLayout.Placeholders.Any(i => i.Name == placeholderName))
			{
				var placeholder = pageLayout.Placeholders.First(i => i.Name == placeholderName);
				canAdd = placeholder.AllowedComponents.Any(i => i == componentName);
			}

			if (canAdd && AddComponent(page, placeholderName, componentName))
			{
				return _pageDataProvider.SavePage(page);
			}

			return false;
		}

		private bool AddComponent(BasePage page, string placeholderName, string componentName)
		{
			string datasourceId = CreateComponentEntry(componentName);
			var placeholder = page.Placeholders?.FirstOrDefault(i => i.Name == placeholderName);
			if (placeholder != null)
			{
				placeholder.Components.Add(new Component
				{
					ComponentId = Guid.NewGuid().ToString(),
					Name = componentName,
					Datasource = datasourceId
				});
			}
			else
			{
				if (page.Placeholders == null)
				{
					page.Placeholders = new List<Placeholder>();
				}
				var newPlaceholder = new Placeholder
				{
					Name = placeholderName,
					Components = new List<Component> { new Component {
						ComponentId = Guid.NewGuid().ToString(),
						Name = componentName,
						Datasource = datasourceId
					}}
				};
				page.Placeholders.Add(newPlaceholder);
			}
			return true;
		}

		private string CreateComponentEntry(string componentName)
		{
			var datasourceId = _componentDataProvider.CreateDatasource(componentName);
			return datasourceId;
		}

		public bool RemoveComponentFromPlaceholder(Guid pageId, string lang, string placeholderName, string componentId)
		{
			var page = (BasePage)_pageDataProvider.GetPage(pageId, lang);
			var placeholder = page.Placeholders.FirstOrDefault(i => i.Name == placeholderName);
			var component = placeholder.Components.FirstOrDefault(i => i.ComponentId == componentId);
			placeholder.Components.Remove(component);

			_componentDataProvider.Delete(Guid.Parse(component.Datasource));

			return _pageDataProvider.SavePage(page);
		}

		public bool SetComponentPosition(Guid pageId, string lang, string placeholderName, string componentId, int newPosition)
		{
			var page = (BasePage)_pageDataProvider.GetPage(pageId, lang);
			var placeholder = page.Placeholders.FirstOrDefault(i => i.Name == placeholderName);
			var component = placeholder.Components.First(i => i.ComponentId == componentId);
			placeholder.Components.Remove(component);
			placeholder.Components.Insert(newPosition, component);
			return _pageDataProvider.SavePage(page);
		}

		public bool SavePage(SaveItemDataModel model, string lang)
		{
			var page = _pageDataProvider.GetPage(model.pageId, lang);

			foreach (var field in model.fields)
			{
				var path = field.fieldPath.Split('.');
				SetFieldValue(page, path.ToList(), field.fieldValue, field.fieldType);
			}

			_pageDataProvider.SavePage(page);
			return true;
		}

		private void SetFieldValue(dynamic target, List<string> path, string value, string fieldType)
		{
			var currentProp = path.First();
			PropertyInfo[] propInfos = target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (var prop in propInfos)
			{
				if (prop.Name.ToLower() == currentProp.ToLower())
				{
					if (path.Count == 1)
					{
						switch (fieldType)
						{
							case "linkfield":
							case "string":
							case "richtext":
								prop.SetValue(target, value);
								break;
							case "boolean":
								prop.SetValue(target, bool.Parse(value));
								break;
							case "guid":
								if (value == "")
								{
									prop.SetValue(target, Guid.Empty);
								}
								else
								{
									prop.SetValue(target, Guid.Parse(value));
								}
								break;
							default:
								break;
						}
					}
					else
					{
						path.Remove(currentProp);
						var nextProp = prop.GetValue(target, null);
						if (nextProp == null)
						{
							nextProp = Activator.CreateInstance(prop.PropertyType);
							prop.SetValue(target, nextProp);
						}
						SetFieldValue(nextProp, path, value, fieldType);
					}
				}
			}
		}

		public List<BaseItem> GetSubItems(Guid parentId)
		{
			List<BaseItem> pages = _pageDataProvider.GetSubpages(parentId).DistinctBy(i => i.GroupId).OrderBy(i=>i.Sort).ToList();
			foreach (var page in pages)
			{
				page.HasSubItems = _pageDataProvider.GetSubpages(page.GroupId).Any();
				page.Type = page.GetType().Name;
				page.InsertOptions = GetInsertOptions(page.Type);
			}
			return pages;
		}

		private List<object> GetInsertOptions(string pageType)
		{
			var result = new List<object>();
			if (!SiteConfiguration.PageTypes.ContainsKey(pageType)) return result;

			foreach (var insertOption in SiteConfiguration.PageTypes[pageType].InsertOptions)
			{
				result.Add(new
				{
					displayName = SiteConfiguration.PageTypes[insertOption].DisplayName,
					insertType = insertOption
				});
			}
			return result;
		}

	}
}
