using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MtcMvcCore.Areas.Admin.Pages.ContentPackages.Controller;
using MtcMvcCore.Core;
using MtcMvcCore.Core.DataProvider.Xml;
using MtcMvcCore.Core.Models;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Areas.Admin.Pages.Common.Controller
{
	[Authorize(Roles = "Administrator, ContentEditor")]
	[ExcludeFromCodeCoverage]
	public class CoreHeaderViewComponent : ViewComponent
	{
		private readonly ILogger<CoreHeaderViewComponent> _logger;
		private readonly IXmlDataProvider _xmlDataProvider;

		public CoreHeaderViewComponent(ILogger<CoreHeaderViewComponent> logger, IXmlDataProvider xmlDataProvider)
		{
			_logger = logger;
			_xmlDataProvider = xmlDataProvider;
		}

		public async Task<IViewComponentResult> InvokeAsync()
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

			await Task.Yield();
			return View($"~/{Settings.PathsCorePath}/Areas/Admin/Pages/Common/Views/Header.cshtml", pages);
		}

	}
}