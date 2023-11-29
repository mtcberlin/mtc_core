using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using MtcMvcCore.Core.Models;
using MtcMvcCore.Core.Models.PageModels;
using MtcMvcCore.Core.Services;
using Xunit;

namespace MtcMvcCore.Core.UnitTests
{
	public class contextServiceTests
	{
		private readonly IContextService _contextService;
		private readonly Mock<IHttpContextAccessor> _httpContext;

		// private readonly SiteMapModel _enSiteMap = new SiteMapModel();
		// private readonly SiteMapModel _defaultSiteMap = new SiteMapModel();

		public contextServiceTests()
		{
			_httpContext = new Mock<IHttpContextAccessor>();
			_contextService = new ContextService(_httpContext.Object);
			// SiteConfiguration.SiteMapModels = new Dictionary<string, SiteMapModel>();
			// SiteConfiguration.SiteMapModels.Add("en", _enSiteMap);
			// SiteConfiguration.SiteMapModels.Add("", _defaultSiteMap);

			SiteConfiguration.PageContextModels = new Dictionary<string, PageContextModel>();
			// SiteConfiguration.PageContextModels.Add("/", new PageContextModel{PageConfigurationModel = new PageConfigurationModel{Information = new Information {Title = "Home"}}});
			// SiteConfiguration.PageContextModels.Add("/list", new PageContextModel { PageConfigurationModel = new PageConfigurationModel { Information = new Information { Title = "List" } } });
			// SiteConfiguration.PageContextModels.Add("/list/details", new PageContextModel { PageConfigurationModel = new PageConfigurationModel { Information = new Information { Title = "Details" } } });
		}
		
		[Fact]
		public void GetSiteMapModelWithEnLangReturnsEnSiteMap()
		{
			_httpContext.Reset();
			_httpContext.Setup(i => i.HttpContext.Request.RouteValues).Returns(new RouteValueDictionary{{"lang", "en"}});

			//Assert.Equal(_enSiteMap, model);
		}

		[Fact]
		public void GetSiteMapModelWithFrLangReturnsDefaultSiteMap()
		{
			_httpContext.Reset();
			_httpContext.Setup(i => i.HttpContext.Request.RouteValues).Returns(new RouteValueDictionary { { "lang", "fr" } });

			//Assert.Equal(_defaultSiteMap, model);
		}

		[Fact]
		public void GetSiteMapModelWithoutLangReturnsDefaultSiteMap()
		{
			_httpContext.Reset();
			_httpContext.Setup(i => i.HttpContext.Request.RouteValues).Returns(new RouteValueDictionary());

			//Assert.Equal(_defaultSiteMap, model);
		}
		
	}
}
