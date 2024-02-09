using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using MtcMvcCore.Core.Constants;
using MtcMvcCore.Core.DataProvider.MongoDb;
using MtcMvcCore.Core.DataProvider.Xml;
using MtcMvcCore.Core.Models.PageModels;
using MtcMvcCore.Core.DataProvider;
using System;
using MtcMvcCore.Core.Models;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core
{
	public abstract class SiteConfiguration
	{
		private static IXmlDataProvider _xmlDataProvider;
		private static IMongoDbDataProvider _mongoDbDataProvider;
		private static IPageDataProvider _pageDataProvider;

		public static Dictionary<string, PageContextModel> PageContextModels =
			new Dictionary<string, PageContextModel>();

		public static Dictionary<string, PageContextModel> StaticPageContextModels =
			new Dictionary<string, PageContextModel>();

		private static Dictionary<string, ComponentConfigModel> _componentConfigModels =
			new Dictionary<string, ComponentConfigModel>();

		public static Dictionary<Guid, PageConfigurationModel> PageConfigModels =
			new Dictionary<Guid, PageConfigurationModel>();
		private static ILogger<SiteConfiguration> _logger;
		public static Dictionary<string, PageTypeConfigModel> PageTypes = new Dictionary<string, PageTypeConfigModel>();
		public static Dictionary<string, LayoutConfigurationModel> Layouts = new Dictionary<string, LayoutConfigurationModel>();
		public static LayoutConfigurationModel[] LayoutKeys;
		public static Dictionary<string, LayoutConfigurationModel> ContentLayouts = new Dictionary<string, LayoutConfigurationModel>();
		public static LayoutConfigurationModel[] ContentLayoutKeys;

		public static void Configure(IXmlDataProvider xmlDataProvider, IMongoDbDataProvider mongoDbDataProvider, ILogger<SiteConfiguration> logger, IPageDataProvider pageDataProvider)
		{
			_xmlDataProvider = xmlDataProvider;
			_mongoDbDataProvider = mongoDbDataProvider;
			_pageDataProvider = pageDataProvider;
			_logger = logger;
		}

		public static void ReadConfigurations()
		{
			PageContextModels = new Dictionary<string, PageContextModel>();
			_componentConfigModels = new Dictionary<string, ComponentConfigModel>();
			PageConfigModels = new Dictionary<Guid, PageConfigurationModel>();
			ReadLayoutConfigurations();
			ReadComponentConfigurations();
			ReadPageConfigurations();
			ReadSiteMaps();
			AddStaticPageContextModels();
		}

		private static void ReadLayoutConfigurations()
		{
			if (Directory.Exists("Data/Settings/Layouts"))
			{
				var layoutFiles = Directory.GetFiles(@"Data/Settings/Layouts", "*", SearchOption.AllDirectories);
				foreach (var layoutFile in layoutFiles)
				{
					var layout = _xmlDataProvider.GetData<LayoutConfigurationModel>(layoutFile);
					Layouts.TryAdd(layout.Key, layout);
				}
			}
			LayoutKeys = Layouts.Values.ToArray();

			if (Directory.Exists("Data/Settings/ContentLayouts"))
			{
				var contentLayoutFiles = Directory.GetFiles(@"Data/Settings/ContentLayouts", "*", SearchOption.AllDirectories);
				foreach (var contentLayout in contentLayoutFiles)
				{
					var layout = _xmlDataProvider.GetData<LayoutConfigurationModel>(contentLayout);
					ContentLayouts.TryAdd(layout.Key, layout);
				}
			}
			ContentLayoutKeys = ContentLayouts.Values.ToArray();

			if (Directory.Exists(@$"{Settings.PathsCorePath}/Core/DefaultSettings/PageTypes"))
			{
				var pageTypeFiles = Directory.GetFiles(@$"{Settings.PathsCorePath}/Core/DefaultSettings/PageTypes", "*", SearchOption.AllDirectories);
				foreach (var pageTypeFile in pageTypeFiles)
				{
					var pageType = _xmlDataProvider.GetData<PageTypeConfigModel>(pageTypeFile);
					PageTypes.TryAdd(pageType.Name, pageType);
				}
			}

			if (Directory.Exists("Data/Settings/PageTypes"))
			{
				var pageTypeFiles = Directory.GetFiles(@"Data/Settings/PageTypes", "*", SearchOption.AllDirectories);
				foreach (var pageTypeFile in pageTypeFiles)
				{
					var pageType = _xmlDataProvider.GetData<PageTypeConfigModel>(pageTypeFile);
					if (PageTypes.ContainsKey(pageType.Name))
					{
						PageTypes[pageType.Name] = pageType;
					}
					else
					{
						PageTypes.TryAdd(pageType.Name, pageType);
					}
				}
			}

		}

		private static void ReadComponentConfigurations()
		{
			var componentFolder = Directory.GetDirectories(@"Components/", "*", SearchOption.TopDirectoryOnly);
			foreach (var component in componentFolder)
			{
				var fullPath = Path.GetFullPath(component).TrimEnd(Path.DirectorySeparatorChar);
				var componentName = fullPath.Split(Path.DirectorySeparatorChar).Last();
				if (File.Exists(@$"{component}/Config.xml"))
				{
					_componentConfigModels.TryAdd(componentName,
						_xmlDataProvider.GetData<ComponentConfigModel>($"{component}/Config.xml"));
					_logger.LogInformation($"Component {componentName} loaded");
				}
			}

			if (!Directory.Exists(@$"{Settings.PathsCorePath}/Components/")) return;

			var adminComponentFolder = Directory.GetDirectories(@$"{Settings.PathsCorePath}/Components/", "*", SearchOption.AllDirectories);
			foreach (var component in adminComponentFolder)
			{
				var fullPath = Path.GetFullPath(component).TrimEnd(Path.DirectorySeparatorChar);
				var componentName = fullPath.Split(Path.DirectorySeparatorChar).Last();
				if (File.Exists(@$"{component}/Config.xml"))
				{
					_componentConfigModels.TryAdd(componentName,
						_xmlDataProvider.GetData<ComponentConfigModel>($"{component}/Config.xml"));
					_logger.LogInformation($"Component {componentName} loaded");
				}
			}
		}

		private static void ReadPageConfigurations()
		{
			if (Settings.DatastoreType == DatastoreType.MONGODB)
			{
				ReadMongoDbPageConfigurations();
			}
			else
			{
				ReadXmlPageConfigurations();
			}
		}

		private static void ReadMongoDbPageConfigurations()
		{
			var pageConfigs = _mongoDbDataProvider.All<PageConfigurationModel>();
			foreach (var pageConfig in pageConfigs)
			{
				PageConfigModels.TryAdd(pageConfig.Id, pageConfig);
			}
		}

		private static void ReadXmlPageConfigurations()
		{
			var pageConfigPaths = Directory.GetFiles(@"Data/PageConfig/", "*.xml", SearchOption.AllDirectories);
			foreach (var pageConfigPath in pageConfigPaths)
			{
				var pageConfig = _xmlDataProvider.GetData<PageConfigurationModel>(pageConfigPath);
				pageConfig.Name = pageConfigPath.Split("/").Last().Replace(".xml", string.Empty);
				PageConfigModels.TryAdd(pageConfig.Id, pageConfig);
			}

			if (!Directory.Exists(@$"{Settings.PathsCorePath}/Data/PageConfig/")) return;
			var adminPageConfigPaths = Directory.GetFiles(@$"{Settings.PathsCorePath}/Data/PageConfig/", "*.xml", SearchOption.AllDirectories);
			foreach (var pageConfigPath in adminPageConfigPaths)
			{
				var pageConfig = _xmlDataProvider.GetData<PageConfigurationModel>(pageConfigPath);
				pageConfig.Name = pageConfigPath.Split("/").Last().Replace(".xml", string.Empty);
				PageConfigModels.TryAdd(pageConfig.Id, pageConfig);
			}
		}

		private static void ReadSiteMaps()
		{
			var pages = _pageDataProvider.AllPages();
			foreach (var baseItem in pages)
			{
				/// TODO:: maybe seoname from default lang
				string url = string.Empty;
				if (baseItem.ParentId == Guids.WebRoot && !PageContextModels.ContainsKey("/") || (PageContextModels.ContainsKey("/") && PageContextModels["/"].Page.GroupId == baseItem.GroupId))
				{
					url = "/";
				}
				else
				{
					url = "/" + baseItem.Name;
				}

				BuildUrl(baseItem, ref url, pages);
				if (!string.IsNullOrEmpty(url))
				{
					var finalUrl = $"/{baseItem.Language}{url.ToLower().Replace(" ", "_")}".TrimEnd('/');

					// Check for duplicates (can happen on adding new page with default name)
					if (!PageContextModels.ContainsKey(finalUrl.ToLower()) && (baseItem.GetType() == typeof(BasePage) || baseItem.GetType().IsSubclassOf(typeof(BasePage))))
					{
						var page = (BasePage)baseItem;
						if (page.Language == Settings.DefaultLanguage)
						{
							PageContextModels.Add(url.ToLower().Replace(" ", "_"), new PageContextModel
							{
								SeoUrlWithoutLang = url.ToLower(),
								Page = page,
								Language = string.Empty
							});
						}

						PageContextModels.Add(finalUrl, new PageContextModel
						{
							SeoUrlWithoutLang = url.ToLower(),
							Page = page,
							Language = page.Language
						});
					}
				}
			}
		}

		private static void AddStaticPageContextModels()
		{
			PageContextModels = PageContextModels.Concat(StaticPageContextModels).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		}

		private static void BuildUrl(BaseItem page, ref string url, IEnumerable<BaseItem> pages)
		{
			if (page.ParentId != Guids.WebRoot)
			{
				var parent = pages.FirstOrDefault(i => i.GroupId == page.ParentId);
				if (parent == null)
				{
					url = string.Empty;
					return;
				}

				if (parent.ParentId != Guids.WebRoot)
				{
					url = "/" + parent.Name + url;
				}

				BuildUrl(parent, ref url, pages);
			}
		}

		private static PageConfigurationModel GetPageConfigModel(Guid configId)
		{
			PageConfigurationModel pageConfig;
			if (PageConfigModels.ContainsKey(configId))
			{
				pageConfig = PageConfigModels[configId];
			}
			else
			{
				pageConfig = new PageConfigurationModel();
				_logger.LogInformation($"No configuration for Page {configId} found.");
			}

			return pageConfig;
		}

		public static ComponentConfigModel GetComponentConfig(string name)
		{
			return _componentConfigModels.ContainsKey(name) ? _componentConfigModels[name] : null;
		}
	}
}