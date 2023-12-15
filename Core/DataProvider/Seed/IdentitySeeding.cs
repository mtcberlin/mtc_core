using System.Linq;
using NLog;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.DataProvider.Seed
{
	public class IdentitySeeding
	{
		private readonly Logger _logger;
		private IUserDataProvider _userDataProvider;

		public IdentitySeeding(IUserDataProvider userDataProvider)
		{
			_logger = LogManager.GetCurrentClassLogger();
			_userDataProvider = userDataProvider;
			Seed();
		}

		private void Seed()
		{
			var roles = _userDataProvider.GetAllRoles();

			if (!roles.Any(i => i.Name == "Administrator"))
			{
				_userDataProvider.CreateRole("Administrator");
			}

			if (!roles.Any(i => i.Name == "SuperEditor"))
			{
				_userDataProvider.CreateRole("SuperEditor");
			}

			if (!roles.Any(i => i.Name == "ContentEditor"))
			{
				_userDataProvider.CreateRole("ContentEditor");
			}

			if (!roles.Any(i => i.Name == "AssetLib"))
			{
				_userDataProvider.CreateRole("AssetLib");
			}

			if (!roles.Any(i => i.Name == "UserManager"))
			{
				_userDataProvider.CreateRole("UserManager");
			}

			if (!roles.Any(i => i.Name == "RoleManager"))
			{
				_userDataProvider.CreateRole("RoleManager");
			}

			if (!roles.Any(i => i.Name == "ContentPackages"))
			{
				_userDataProvider.CreateRole("ContentPackages");
			}

			if (!roles.Any(i => i.Name == "User"))
			{
				_userDataProvider.CreateRole("User");
			}

			var admin = _userDataProvider.GetUserByName("admin");

			if (admin == null)
			{
				_userDataProvider.Create("admin", "Admin1234=", new[] { "Administrator", "SuperEditor", "ContentEditor", "User" }, true);
			}
		}

	}
}
