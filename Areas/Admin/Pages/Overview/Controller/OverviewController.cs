using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MtcMvcCore.Areas.Admin.Pages.ContentPackages.Controller;
using MtcMvcCore.Core;
using MtcMvcCore.Core.DataProvider.Xml;
using MtcMvcCore.Core.Models;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Areas.Admin.Pages.Overview.Controller
{
	[Authorize]
	[ExcludeFromCodeCoverage]
	public class OverviewController : AdminBaseController
	{
		private readonly ILogger<OverviewController> _logger;
		private readonly IXmlDataProvider _xmlDataProvider;

		public OverviewController(ILogger<OverviewController> logger, IXmlDataProvider xmlDataProvider)
		{
			_logger = logger;
			_xmlDataProvider = xmlDataProvider;
		}

		[Route("admin/overview")]
		public IActionResult Index()
		{
			List<AdminPageConfigurationModel> pages = new List<AdminPageConfigurationModel>();
			var adminPages = Directory.GetFiles(@$"{Settings.PathsCorePath}/Areas/Admin/Pages", "Config.xml", SearchOption.AllDirectories);
			foreach (var adminPage in adminPages)
			{
				var config = _xmlDataProvider.GetData<AdminPageConfigurationModel>(adminPage);
				if (string.IsNullOrEmpty(config.Roles))
				{
					pages.Add(config);
				} else {
					var roles = config.Roles.Split(',');
					foreach(var role in roles) {
						if(User.IsInRole(role)) {
							pages.Add(config);
							break;
						}
					}
				}
			}

			adminPages = Directory.GetFiles(@$"Areas/Admin/Pages", "Config.xml", SearchOption.AllDirectories);
			foreach (var adminPage in adminPages)
			{
				var config = _xmlDataProvider.GetData<AdminPageConfigurationModel>(adminPage);
				if (string.IsNullOrEmpty(config.Roles))
				{
					pages.Add(config);
				} else {
					var roles = config.Roles.Split(',');
					foreach(var role in roles) {
						if(User.IsInRole(role)) {
							pages.Add(config);
							break;
						}
					}
				}
			}

			var grouped = pages.GroupBy(i => i.Group);

			return View($"~/{Settings.PathsCorePath}/Areas/Admin/Pages/Overview/Views/Overview.cshtml", grouped);
		}

	}
}