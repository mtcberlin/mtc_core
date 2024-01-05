using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MtcMvcCore.Core.DataProvider.Xml;
using MtcMvcCore.Core.Models.Authentication;
using NLog;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.DataProvider.MongoDb
{
	public class MongoDbUserDataProvider : IUserDataProvider
	{
		private readonly IHttpContextAccessor _httpContext;
		private readonly IXmlDataProvider _dataService;
		private readonly SignInManager<MongoDbUserModel> _signInManager;
		private readonly UserManager<MongoDbUserModel> _userManager;
		private readonly RoleManager<MongoDbRole> _roleManager;
		private readonly Logger _logger;

		public MongoDbUserDataProvider(IHttpContextAccessor httpContext, IXmlDataProvider dataService, SignInManager<MongoDbUserModel> signInManager, UserManager<MongoDbUserModel> userManager, RoleManager<MongoDbRole> roleManager)
		{
			_logger = LogManager.GetCurrentClassLogger();
			_httpContext = httpContext;
			_dataService = dataService;
			_signInManager = signInManager;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		public async Task<bool> SignInUser(string username, string password, bool isPersistent)
		{
			var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent, lockoutOnFailure: false);
			return result.Succeeded;
		}

		public async Task<bool> LogOutUser()
		{
			await _signInManager.SignOutAsync();
			return true;
		}

		public UserRegisterResult Create(string username, string password, string[] roles, bool activate = false)
		{
			var user = new MongoDbUserModel { UserName = username, Email = username, EmailConfirmed = activate };
			var result = _userManager.CreateAsync(user, password).Result;
			if (result.Succeeded)
			{
				_userManager.AddToRolesAsync(user, roles);
				return new UserRegisterResult
				{
					UserId = user.Id
				};
			}

			return new UserRegisterResult
			{
				ErrorCodes = result.Errors.Select(i => i.Code).ToList()
			};
		}

		public string GetPasswortResetToken(string username)
		{
			var user = _userManager.FindByEmailAsync(username).Result;
			var result = _userManager.GeneratePasswordResetTokenAsync(user).Result;
			return result;
		}

		public UserRegisterResult ResetPassword(string username, string token, string password)
		{
			var dbUser = _userManager.FindByEmailAsync(username).Result;
			var result = _userManager.ResetPasswordAsync(dbUser, token, password).Result;
			if (result.Succeeded)
			{
				return new UserRegisterResult
				{
					UserId = dbUser.Id
				};
			}

			return new UserRegisterResult
			{
				ErrorCodes = result.Errors.Select(i => i.Code).ToList()
			};
		}

		public UserModel GetCurrentUser()
		{
			var user = _userManager.FindByIdAsync(_httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value).Result;
			return UserModel.FromMongoUser(user);
		}

		public void SetUsers(List<UserModel> users)
		{
			// _registeredUsers.UserModels = users;
			// _dataService.SetData(_registeredUsers, "Data/Administration/Users/Users.xml");
		}

		private async Task<string> GetRoleName(string objectId)
		{
			var role = await _roleManager.FindByIdAsync(objectId);
			return role.Name;
		}

		#region User

		public List<UserModel> GetAllUsers()
		{
			var users = _userManager.Users.Select(i => new UserModel
			{
				IsActive = i.EmailConfirmed,
				UserId = i.Id,
				UserName = i.UserName,
				FirstName = i.FirstName,
				LastName = i.LastName,
				Email = i.Email,
				Roles = i.Roles
			}).ToList();

			foreach (var user in users)
			{
				user.Roles = user.Roles.Select(i => GetRoleName(i).Result).ToList();
			}

			return users;
		}

		public UserModel GetUserById(string userId)
		{
			var user = _userManager.FindByIdAsync(userId).Result;
			if (user == null) return null;
			var userModel = UserModel.FromMongoUser(user);
			userModel.Roles = user.Roles.Select(i => GetRoleName(i).Result).ToList();
			return userModel;
		}

		public UserModel GetUserByName(string userName)
		{
			var user = _userManager.FindByNameAsync(userName).Result;
			if (user == null) return null;
			var userModel = UserModel.FromMongoUser(user);
			userModel.Roles = user.Roles.Select(i => GetRoleName(i).Result).ToList();
			return userModel;
		}

		public bool UpdateUser(UserModel user)
		{
			try
			{
				var dbUser = _userManager.FindByIdAsync(user.UserId.ToString()).Result;
				dbUser.EmailConfirmed = user.IsActive;
				dbUser.UserName = user.UserName;
				dbUser.FirstName = user.FirstName;
				dbUser.LastName = user.LastName;
				dbUser.Email = user.Email;
				var result = _userManager.UpdateAsync(dbUser).Result;
				if (result.Succeeded)
				{
					var dbRoles = dbUser.Roles.Select(i => GetRoleName(i).Result).ToList();
					var toRemove = dbRoles.Except(user.Roles);
					var roleResult = _userManager.RemoveFromRolesAsync(dbUser, toRemove).Result;

					var toAdd = user.Roles.Except(dbRoles);
					roleResult = _userManager.AddToRolesAsync(dbUser, toAdd).Result;

				}
				return true;
			}
			catch (Exception e)
			{
				_logger.Error(e);
			}
			return false;
		}

		public bool RemoveUser(string userId)
		{
			var user = _userManager.FindByIdAsync(userId).Result;
			var result = _userManager.DeleteAsync(user).Result;
			return result.Succeeded;
		}

		public bool CreateUser(string userName, string firstName, string lastName, string email, string roles)
		{
			var user = new MongoDbUserModel { Id = Guid.NewGuid(), UserName = userName, FirstName = firstName, LastName = lastName, Email = email };
			var result = _userManager.CreateAsync(user, "Start1234=").Result;
			if (result.Succeeded)
			{
				foreach (var role in roles.Split(","))
				{
					var roleAddResult = _userManager.AddToRoleAsync(user, role.Trim()).Result;
				}
			}

			return result.Succeeded;
		}

		#endregion
		#region Roles
		public List<RoleModel> GetAllRoles()
		{
			var roles = _roleManager.Roles.Select(RoleModel.FromMongoUser).ToList();
			return roles;
		}

		public bool UpdateRole(string roleId, string roleName)
		{
			try
			{
				var role = _roleManager.FindByIdAsync(roleId).Result;
				if (role != null)
				{
					role.Name = roleName;
					var result = _roleManager.UpdateAsync(role).Result;
					return result.Succeeded;
				}
			}
			catch (Exception e)
			{
				_logger.Error(e);
			}
			return false;
		}

		public bool RemoveRole(string roleId)
		{
			try
			{
				var role = _roleManager.FindByIdAsync(roleId).Result;
				if (role != null)
				{
					var result = _roleManager.DeleteAsync(role).Result;
					return result.Succeeded;
				}
			}
			catch (Exception e)
			{
				_logger.Error(e);
			}
			return false;
		}

		public bool CreateRole(string roleName)
		{
			try
			{
				var result = _roleManager.CreateAsync(new MongoDbRole
				{
					Id = Guid.NewGuid(),
					Name = roleName
				}).Result;
				return result.Succeeded;
			}
			catch (Exception e)
			{
				_logger.Error(e);
			}
			return false;
		}

		public string GenerateMailConfirmToken(Guid userId)
		{
			var user = _userManager.FindByIdAsync(userId.ToString()).Result;
			return _userManager.GenerateEmailConfirmationTokenAsync(user).Result;
		}

		public bool ConfirmMail(string userId, string token)
		{
			var user = _userManager.FindByIdAsync(userId).Result;
			var result = _userManager.ConfirmEmailAsync(user, token).Result;
			return result.Succeeded;
		}

		public bool AddUserProfileValue(string key, string value)
		{
			var user = _userManager.FindByIdAsync(_httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value).Result;
			if (user.UserProfile.Values == null)
			{
				user.UserProfile.Values = new Dictionary<string, string>();
			}
			if (!user.UserProfile.Values.ContainsKey(key))
			{
				user.UserProfile.Values.Add(key, value);
			}
			else
			{
				user.UserProfile.Values[key] = value;
			}

			var result = _userManager.UpdateAsync(user).Result;

			return result.Succeeded;
		}

		public string GetUserProfileValue(string key)
		{
			var user = _userManager.FindByIdAsync(_httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value).Result;
			if (user.UserProfile.Values == null)
			{
				return null;
			}
			if (user.UserProfile.Values.ContainsKey(key))
			{
				return user.UserProfile.Values[key];
			}

			return null;
		}


		public bool SaveUserData(UserModel user, string firstName, string lastName, string email)
		{
			var dbUser = _userManager.FindByIdAsync(user.UserId.ToString()).Result;
			dbUser.FirstName = firstName;
			dbUser.LastName = lastName;

			var result = _userManager.UpdateAsync(dbUser).Result;

			return result.Succeeded;
		}

		public IdentityResult ChangePasswort(UserModel user, string currentpasswort, string newPasswort)
		{
			var dbUser = _userManager.FindByIdAsync(user.UserId.ToString()).Result;
			var result = _userManager.ChangePasswordAsync(dbUser, currentpasswort, newPasswort).Result;
			return result;
		}

		public bool SetNewPassword(UserModel user, string newPassword)
		{
			var dbUser = _userManager.FindByIdAsync(user.UserId.ToString()).Result;
			var token = _userManager.GeneratePasswordResetTokenAsync(dbUser).Result;
			var result = _userManager.ResetPasswordAsync(dbUser, token, newPassword).Result;
			return result.Succeeded;
		}

		#endregion
	}
}
