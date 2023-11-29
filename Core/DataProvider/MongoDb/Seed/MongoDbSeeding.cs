using System;
using System.Collections.Generic;
using MtcMvcCore.Core.Constants;
using MtcMvcCore.Core.Models.Media;
using MtcMvcCore.Core.Models.PageModels;
using NLog;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.DataProvider.MongoDb.Seed
{
	public class MongoDbSeeding
	{
		private readonly Logger _logger;
		private IMongoDbDataProvider _dataProvider;

		public MongoDbSeeding(IMongoDbDataProvider dataProvider)
		{
			_logger = LogManager.GetCurrentClassLogger();
			_dataProvider = dataProvider;
			SeedBaseStructure();
			Seed();
		}

		private void SeedBaseStructure()
		{
			var result = _dataProvider.Get<BaseItem, Guid>("Id", Guids.PageRoot);

			if (result.Id == Guid.Empty)
			{
				var root = new BaseItem
				{
					Id = Guids.PageRoot,
					GroupId = Guids.PageRoot,
					Name = "Core Root",
					Language = Settings.DefaultLanguage,
					Version = 1,
					Created = DateTime.Now,
					Updated = DateTime.Now
				};
				_dataProvider.Create(root);
			}

			var web = _dataProvider.Get<BaseItem, Guid>("Id", Guids.WebRoot);

			if (web.Id == Guid.Empty)
			{
				var webFolder = new WebRoot
				{
					Id = Guids.WebRoot,
					GroupId = Guids.WebRoot,
					Name = "Web",
					Language = Settings.DefaultLanguage,
					Version = 1,
					ParentId = Guids.PageRoot,
					Created = DateTime.Now,
					Updated = DateTime.Now
				};
				_dataProvider.Create(webFolder);
			}
		}

		private void Seed()
		{
			var result = _dataProvider.Get<CoreMediaFolder, Guid>("Id", Guids.MediaRoot);

			if (result.Id == Guid.Empty)
			{
				var home = _dataProvider.Get<BasePage, Guid>("ParentId", Guids.PageRoot);
				Guid homeGuid = home.Id;
				if (home.ParentId == Guid.Empty)
				{
					homeGuid = SeedDefaultHome();
				}

				var mediaRoot = new CoreMediaFolder
				{
					Id = Guids.MediaRoot,
					ParentId = Guids.PageRoot,
					Name = "Root"
				};
				_dataProvider.Create(mediaRoot);

			}

		}

		private Guid SeedDefaultHome()
		{
			var result = _dataProvider.Get<BasePage, Guid>("ParentId", Guids.WebRoot);

			if (result.Id == Guid.Empty)
			{
				var pageId = Guid.NewGuid();

				var homePage = new BasePage
				{
					Id = Guid.NewGuid(),
					GroupId = pageId,
					Name = "Home",
					Action = "Index",
					Controller = "Default",
					Language = "de",
					Layout = "_LayoutWithFooter",
					View = "1_c_12",
					Title = "Seeded Home",
					Published = true,
					ParentId = Guids.WebRoot
				};
				_dataProvider.Create(homePage);

				var homePageEn = new BasePage
				{
					Id = Guid.NewGuid(),
					GroupId = pageId,
					Name = "Home",
					Action = "Index",
					Controller = "Default",
					Language = "de",
					Layout = "_LayoutWithFooter",
					View = "1_c_12",
					Title = "Seeded Home",
					Published = true,
					ParentId = Guids.WebRoot
				};
				_dataProvider.Create(homePageEn);

				return homePage.Id;
			}
			return Guid.Empty;
		}
	}
}
