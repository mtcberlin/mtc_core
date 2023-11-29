using System;
using System.Collections.Generic;
using System.Linq;
using MtcMvcCore.Core.DataProvider.Xml.Models;
using MtcMvcCore.Core.Models;
using MtcMvcCore.Core.Models.PageModels;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.DataProvider.Xml
{

	public class XmlPageDataProvider : IPageDataProvider
	{
		private IXmlDataProvider _dataProvider;

		public XmlPageDataProvider(IXmlDataProvider dataProvider)
		{
			_dataProvider = dataProvider;
		}

		public void AddPage(Guid parentId, string lang, string pageTypeName)
		{
			throw new NotImplementedException();
		}

		public Core.Models.PageModels.BasePage AddVersion(Guid pageId, string type, string language)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Core.Models.PageModels.BaseItem> AllPages()
		{
			var result = new List<Core.Models.PageModels.BasePage>();
			var siteMap = _dataProvider.GetData<SiteMapModel>(@"Data/SiteMap.xml");
			foreach (var page in siteMap.Pages)
			{
				TransformXmlPagesToApplicationPages(page, result, Guid.Parse("{11111111-1111-1111-1111-111111111111}"));
			}
			return result;
		}

		private void TransformXmlPagesToApplicationPages(Models.Page currentPage, List<Core.Models.PageModels.BasePage> resultList, Guid parentId)
		{
			//Ensure every Page has an Id
			if(currentPage.Id == Guid.Empty) {
				currentPage.Id = Guid.NewGuid();
			}
			
			
				var result = new Core.Models.PageModels.BasePage();
					result.Name = currentPage.Name;
					result.Action = currentPage.Action;
					result.Controller = currentPage.Controller;
					result.Area = currentPage.Area;
					result.Id = currentPage.Id;
					result.ParentId = parentId;
					// result.Language = new LanguageVersion
					// {
					// 	IsoCode = currentPage.IsoCode ?? Settings.DefaultLanguage,
					// 	SeoName = currentPage.SeoName,
					// 	UrlParameterName = currentPage.UrlParameterName
					// };
					// result.Version = new ContentVersion
					// {
					// 	DoNotShowInNavigation = currentPage.DoNotShowInNavigation,
					// 	PageconfigId = GetPageConfigIdByName(currentPage.PageConfiguration),
					// 	Redirect = currentPage.Redirect,
					// 	VersionsNummer = 1
					// };
					// result.Versions = new []{1};
					resultList.Add(result);
			

			foreach (var page in currentPage.Pages)
			{
				TransformXmlPagesToApplicationPages(page, resultList, currentPage.Id);
			}
		}

		private Guid GetPageConfigIdByName(string name) {
			var config = SiteConfiguration.PageConfigModels.FirstOrDefault(i =>i.Value.Name == name);
			if(config.Key != Guid.Empty) {
				return config.Key;
			}
			return Guid.Empty;
		}

		public void DeletePage(Guid pageId)
		{
			throw new NotImplementedException();
		}

		public PageConfigurationModel GetPageConfiguration(Guid pageConfigurationId)
		{
			throw new NotImplementedException();
		}

		public Core.Models.PageModels.BasePage GetPageProperties(Guid pageId, string lang)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Core.Models.PageModels.BaseItem> PagesForTreeView()
		{
			throw new NotImplementedException();
		}

		public bool SavePage(Core.Models.PageModels.BaseItem model)
		{
			throw new NotImplementedException();
		}

		public bool SavePage(Core.Models.PageModels.BasePage model, string lang)
		{
			throw new NotImplementedException();
		}

		public Core.Models.PageModels.BaseItem GetPage(Guid pageId, string lang)
		{
			throw new NotImplementedException();
		}

		public List<BasePage> GetPages(Guid pageId, bool withSubpages)
		{
			throw new NotImplementedException();
		}

		public List<EditorFieldDefinition> GetPageFieldDefinitions(Guid pageId, string type, string lang)
		{
			throw new NotImplementedException();
		}

		public bool SavePages(object pageData, bool checkParent)
		{
			throw new NotImplementedException();
		}

		public List<Core.Models.PageModels.BaseItem> GetSubpages(Guid parentId)
		{
			throw new NotImplementedException();
		}

		public PageConfigurationModel GetPageConfiguration(Guid pageId, string lang)
		{
			throw new NotImplementedException();
		}

		public string[] GetLanguages(Guid pageId)
		{
			throw new NotImplementedException();
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return base.ToString();
		}

		public BasePage AddLanguage(Guid pageId, string lang)
		{
			throw new NotImplementedException();
		}

	}

}
