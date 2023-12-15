using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MtcMvcCore.Core.Models.Authentication;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.DataProvider
{
	public interface IUserDataProvider
	{
		Task<bool> SignInUser(string username, string password, bool isPersistent);

		Task<bool> LogOutUser();

		/// <summary>
		/// Creates a User and returnts its UserId for verification
		/// </summary>
		/// <param></param>
		/// <returns>Guid</returns>
		UserRegisterResult Create(string username, string password, string[] roles, bool activate = false);

		[Obsolete("Just for xml handle different")]
		void SetUsers(List<UserModel> users);

		/// <summary>
		/// returns all stored users
		/// </summary>
		/// <param></param>
		/// <returns>List of UserModel</returns>
		List<UserModel> GetAllUsers();
		
		/// <summary>
		/// returns the current user 
		/// </summary>
		/// <param>string</param>
		/// <returns>UserModel</returns>
		UserModel GetCurrentUser();

		/// <summary>
		/// returns the user by given id
		/// </summary>
		/// <param>string</param>
		/// <returns>UserModel</returns>
		UserModel GetUserById(string userId);
		
		/// <summary>
		/// returns the user by given name
		/// </summary>
		/// <param>string</param>
		/// <returns>UserModel</returns>
		UserModel GetUserByName(string userName);

		/// <summary>
		/// returns the reset token for the given user
		/// </summary>
		/// <param>string email</param>
		/// <returns>string</returns>
		string GetPasswortResetToken(string email);

		/// <summary>
		/// Updated one user by given id
		/// </summary>
		/// <param>UserModel</param>
		/// <returns>bool</returns>
		bool UpdateUser(UserModel user);
		
		/// <summary>
		/// Updated one users password by given id
		/// </summary>
		/// <param>UserModel</param>
		/// <returns>bool</returns>
		bool SetNewPassword(UserModel user, string newPassword);

		/// <summary>
		/// Updated the name and email
		/// </summary>
		/// <param>UserModel</param>
		/// <returns>bool</returns>
		bool SaveUserData(UserModel user, string firstName, string lastName, string email);

		/// <summary>
		/// Updated the password for the current user
		/// </summary>
		/// <param>UserModel</param>
		/// <returns>IdentityResult</returns>
		IdentityResult ChangePasswort(UserModel user, string currentpasswort, string newPasswort);

		/// <summary>
		/// Removes one user by given id
		/// </summary>
		/// <param>UserModel</param>
		/// <returns>bool</returns>
		bool RemoveUser(string userId);

		/// <summary>
		/// Create a new user
		/// </summary>
		/// <param>roleName</param>
		/// <returns></returns>
		bool CreateUser(string userName, string firstName, string lastName, string email, string roles);

		/// <summary>
		/// returns all stored roles
		/// </summary>
		/// <param></param>
		/// <returns>List of RoleModel</returns>
		List<RoleModel> GetAllRoles();

		/// <summary>
		/// Updated one role name by given id
		/// </summary>
		/// <param>roleId</param>
		/// <param>roleName</param>
		/// <returns></returns>
		bool UpdateRole(string roleId, string roleName);

		/// <summary>
		/// Removes one role by given id
		/// </summary>
		/// <param>roleId</param>
		/// <returns></returns>
		bool RemoveRole(string roleId);

		/// <summary>
		/// Create a new role
		/// </summary>
		/// <param>roleName</param>
		/// <returns></returns>
		bool CreateRole(string roleName);
		
		/// <summary>
		/// Create a new token to confirm the Mail-Address
		/// </summary>
		/// <param>userId</param>
		/// <returns>string token</returns>
		string GenerateMailConfirmToken(Guid userId);
		
		/// <summary>
		/// Check token to confirm the Mail-Address
		/// </summary>
		/// <param>userId, token</param>
		/// <returns>bool</returns>
		bool ConfirmMail(string userid, string token);

		/// <summary>
		/// Update UserProfile
		/// </summary>
		/// <param>key, value</param>
		/// <returns>bool</returns>
		bool AddUserProfileValue(string key, string value);

		/// <summary>
		/// Update UserProfile
		/// </summary>
		/// <param>key</param>
		/// <returns>string value</returns>
		string GetUserProfileValue(string key);
	}
}
