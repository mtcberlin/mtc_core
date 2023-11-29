using Microsoft.AspNetCore.Mvc;
using MtcMvcCore.Core.DataProvider;

namespace MtcMvcCore.Areas.Admin.Pages.Common.Controller
{
	public class CoreHeaderMvcController : Microsoft.AspNetCore.Mvc.Controller
	{
		IUserDataProvider _userDataProvider;
		public CoreHeaderMvcController(IUserDataProvider userDataProvider)
		{
			_userDataProvider = userDataProvider;
		}

		[HttpGet]
		[Route("api/header/logout")]
		public IActionResult Logout()
		{
			_userDataProvider.LogOutUser();
			return Redirect("/");
		}

	}
}
