using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MtcMvcCore.Areas.Admin.Pages.ContentPackages.Controller;
using MtcMvcCore.Areas.Admin.Pages.UserManager.Models;
using MtcMvcCore.Core;
using MtcMvcCore.Core.DataProvider;
using MtcMvcCore.Core.Models.Authentication;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Areas.Admin.Pages.UserManager.Controller
{
	[Authorize(Roles = "Administrator, UserManager")]
	public class UserManagerController : AdminBaseController
	{
		private readonly ILogger<UserManagerController> _logger;
		private readonly IUserDataProvider _userDataProvider;

		public UserManagerController(ILogger<UserManagerController> logger, IUserDataProvider userDataProvider)
		{
			_logger = logger;
			_userDataProvider = userDataProvider;
		}

		[Route("admin/usermanager")]
		public IActionResult Index()
		{
			var users = _userDataProvider.GetAllUsers();
			return View($"~/{Settings.PathsCorePath}/Areas/Admin/Pages/UserManager/Views/UserManager.cshtml", users);
		}

		[HttpGet]
		[Route("api/core/user/remove")]
		public IActionResult RemoveUser(string userId)
		{
			_userDataProvider.RemoveUser(userId);
			return Redirect("/admin/usermanager");
		}

		[HttpPost]
		[Route("api/core/user/update")]
		public IActionResult EditUser([FromBody] UserViewModel userViewModel)
		{
			var user = CreateUserModel(userViewModel.UserId, userViewModel.UserName, userViewModel.FirstName, userViewModel.LastName, userViewModel.Email, userViewModel.Roles, userViewModel.IsActive);
			_userDataProvider.UpdateUser(user);
			if(!string.IsNullOrEmpty(userViewModel.NewPw)) {
				_userDataProvider.SetNewPassword(user, userViewModel.NewPw);
			}
			return Redirect("/admin/usermanager");
		}

		[HttpPost]
		[Route("api/core/user/add")]
		public IActionResult AddUser(string userNameAdd, string firstNameAdd, string lastNameAdd, string emailAdd, string rolesAdd)
		{
			_userDataProvider.CreateUser(userNameAdd, firstNameAdd, lastNameAdd, emailAdd, rolesAdd);
			return Redirect("/admin/usermanager");
		}

		private UserModel CreateUserModel(string userId, string userName, string firstName, string lastName, string email, string rolesText, bool isActive)
		{
			return new UserModel
			{
				IsActive = isActive,
				UserId = Guid.Parse(userId),
				UserName = userName,
				FirstName = firstName,
				LastName = lastName,
				Email = email,
				RolesText = rolesText
			};
		}

	}
}