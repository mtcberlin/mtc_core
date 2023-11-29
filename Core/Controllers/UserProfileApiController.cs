using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using MtcMvcCore.Core.DataProvider;
using System.Collections.Generic;

namespace MtcMvcCore.Core.Controllers
{
	[ExcludeFromCodeCoverage]
	public class UserProfileApiController : CoreBaseController
	{
		private readonly ILogger<UserProfileApiController> _logger;
		private readonly IUserDataProvider _userDataProvider;

		public UserProfileApiController(ILogger<UserProfileApiController> logger, IUserDataProvider userDataProvider)
		{
			_logger = logger;
			_userDataProvider = userDataProvider;
		}

		[Route("/api/userprofile/customdata/set")]
		[HttpPost]
		public IActionResult SetCustomData([FromBody] Dictionary<string, string> data)
		{
			var success = true;
			foreach(var pair in data) {
				success = success && _userDataProvider.AddUserProfileValue(pair.Key, pair.Value);
			}

			return Json(new { success = success });
		}

		[Route("/api/userprofile/customdata/get")]
		[HttpGet]
		public IActionResult GetCustomData(string key)
		{
			var data = _userDataProvider.GetUserProfileValue(key);
			return Json(new { data });
		}
	}
}
