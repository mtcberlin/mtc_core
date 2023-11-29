using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MtcMvcCore.Areas.Admin.Pages.ContentPackages.Controller;
using MtcMvcCore.Core;
using MtcMvcCore.Core.DataProvider;
using MtcMvcCore.Core.Services;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Areas.Admin.Pages.RoleManager.Controller
{
	[Authorize(Roles = "Administrator, RoleManager")]
	public class RoleManagerController : AdminBaseController
	{
		private readonly ILogger<RoleManagerController> _logger;
		private readonly IUserDataProvider _userDataProvider;

		public RoleManagerController(ILogger<RoleManagerController> logger, IUserDataProvider userDataProvider)
		{
			_logger = logger;
			_userDataProvider = userDataProvider;
		}

		[Route("admin/rolemanager")]
		public IActionResult Index()
		{
			var roles = _userDataProvider.GetAllRoles();
			return View($"~/{Settings.PathsCorePath}/Areas/Admin/Pages/RoleManager/Views/RoleManager.cshtml", roles);
		}

		[HttpGet]
		[Route("admin/rolemanager/{roleId}")]
		public IActionResult Edit(string roleId)
		{
			var role = _userDataProvider.GetAllRoles();
			return View($"~/{Settings.PathsCorePath}/Areas/Admin/Pages/RoleManager/Views/RoleEdit.cshtml", role);
		}

		[HttpGet]
		[Route("api/core/role/remove")]
		public IActionResult RemoveRole(string roleId)
		{
			_userDataProvider.RemoveRole(roleId);
			return Redirect("/admin/rolemanager");
		}

		[HttpPost]
		[Route("api/core/role/edit")]
		public IActionResult EditRole(int position, string[] roleId, string[] roleName)
		{
			_userDataProvider.UpdateRole(roleId[position], roleName[position]);
			return Redirect("/admin/rolemanager");
		}

		[HttpPost]
		[Route("api/core/role/add")]
		public IActionResult AddRole(string roleNameAdd)
		{
			_userDataProvider.CreateRole(roleNameAdd);
			return Redirect("/admin/rolemanager");
		}

	}
}