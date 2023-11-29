using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MtcMvcCore.Areas.Admin.Pages.ContentPackages.Controller;
using MtcMvcCore.Core;
using MtcMvcCore.Core.Controllers;
using MtcMvcCore.Core.DataProvider;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Areas.Admin.Pages.Login.Controller
{
	[ExcludeFromCodeCoverage]
	public class LoginController : AdminBaseController
	{
		private readonly ILogger<LoginController> _logger;
		private readonly IUserDataProvider _userDataProvider;

		public LoginController(ILogger<LoginController> logger, IUserDataProvider userDataProvider)
		{
			_logger = logger;
			_userDataProvider = userDataProvider;
		}
		
		[HttpGet]
		[Route("admin/login")]
		public IActionResult Login()
		{
			return View($"~/{Settings.PathsCorePath}/Areas/Admin/Pages/Login/Views/Login.cshtml");
		}

		[HttpPost]
		[Route("admin/login")]
		public async Task<IActionResult> Login(string username, string password, string returnUrl)
		{
			var result = await _userDataProvider.SignInUser(username, password, true);
			if (result)
			{
				return Redirect(returnUrl ?? "overview");
			}

			return View($"~/{Settings.PathsCorePath}/Areas/Admin/Pages/Login/Views/Login.cshtml");
		}

		[HttpPost]
		[Route("admin/reg")]
		public async Task<IActionResult> Reg(string returnUrl)
		{
			// var result = _userDataProvider.Create("admin", "Admin1234=");
			// if (result)
			// {
			// 	return Redirect(returnUrl ?? "overview");
			// }

			await Task.Yield();
			return View($"~/{Settings.PathsCorePath}/Areas/Admin/Pages/Login/Views/Login.cshtml");
		}

	}
}