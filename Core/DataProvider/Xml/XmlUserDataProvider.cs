using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MtcMvcCore.Core.Models.Authentication;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.DataProvider.Xml
{
	public class XmlUserDataProvider : IXmlUserDataProvider
	{
		private readonly IHttpContextAccessor _httpContext;
		private readonly IXmlDataProvider _dataService;
		private UserCollection _registeredUsers;

		public XmlUserDataProvider(IHttpContextAccessor httpContext, IXmlDataProvider dataService)
		{
			_httpContext = httpContext;
			_dataService = dataService;
		}

		public async Task<bool> SignInUser(string username, string password, bool isPersistent)
		{
			if (CheckUserData(username, password))
			{
				var userModel = _registeredUsers.UserModels.First(i => i.UserName == username);
				await CreateUserTicket(userModel, isPersistent);
				return true;
			}

			return false;
		}

		public async Task<bool> LogOutUser()
		{
			// Todo Logout USer
			await Task.Yield();
			return true;
		}


		public UserRegisterResult Create(string username, string password, string[] roles)
		{
			throw new NotImplementedException();
		}

		private bool CheckUserData(string username, string password)
		{
			if (_registeredUsers == null)
			{
				ReadUserData();
			}

			return _registeredUsers.UserModels.Any(i => i.UserName == username && i.Password == password);
		}

		private void ReadUserData()
		{
			_registeredUsers = _dataService.GetData<UserCollection>("Data/Administration/Users/Users.xml");
		}

		public List<UserModel> GetAllUsers()
		{
			ReadUserData();
			return _registeredUsers.UserModels;
		}

		public List<RoleModel> GetAllRoles()
		{
			throw new NotImplementedException();
		}

		private async Task CreateUserTicket(UserModel userModel, bool isPersistent)
		{
			var claims = new List<Claim>();

			// Setting  
			claims.Add(new Claim(ClaimTypes.Name, userModel.UserName));
			foreach (var userRole in userModel.Roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, userRole));
			}

			var claimIdentities = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var claimPrincipal = new ClaimsPrincipal(claimIdentities);
			var authenticationManager = _httpContext.HttpContext;

			// Sign In.  
			await authenticationManager.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal, new AuthenticationProperties() { IsPersistent = isPersistent });

		}

		public void SetUsers(List<UserModel> users)
		{
			_registeredUsers.UserModels = users;
			_dataService.SetData(_registeredUsers, "Data/Administration/Users/Users.xml");
		}

		public UserModel GetUserById(string userId)
		{
			throw new NotImplementedException();
		}

		public bool UpdateUser(UserModel user)
		{
			throw new NotImplementedException();
		}

		public bool RemoveUser(string userId)
		{
			throw new NotImplementedException();
		}

		public bool CreateUser(string userName, string firstName, string lastName, string email, string roles)
		{
			throw new NotImplementedException();
		}

		public bool UpdateRole(string roleId, string roleName)
		{
			throw new NotImplementedException();
		}

		public bool RemoveRole(string roleId)
		{
			throw new NotImplementedException();
		}

		public bool CreateRole(string roleName)
		{
			throw new NotImplementedException();
		}

		public UserModel GetUserByName(string userName)
		{
			throw new NotImplementedException();
		}

		public string GenerateMailConfirmToken(Guid userId)
		{
			throw new NotImplementedException();
		}

		public bool ConfirmMail(string userid, string token)
		{
			throw new NotImplementedException();
		}

		public bool AddUserProfileValue(string key, string value)
		{
			throw new NotImplementedException();
		}

		public string GetUserProfileValue(string key)
		{
			throw new NotImplementedException();
		}
		public bool SaveUserData(UserModel user, string firstName, string lastName, string email){
			throw new NotImplementedException();
		}


		public IdentityResult ChangePasswort(UserModel user, string currentpasswort, string newPasswort)
		{
			throw new NotImplementedException();
		}

        public UserModel GetCurrentUser()
        {
            throw new NotImplementedException();
        }

		public string GetPasswortResetToken(string email)
		{
			throw new NotImplementedException();
		}
	}
}
