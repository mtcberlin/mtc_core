using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using MtcMvcCore.Core;
using MtcMvcCore.Core.Caching;
using MtcMvcCore.Core.Models;
using MtcMvcCore.Core.Services;

namespace MtcMvcCore.Core.Controllers
{
	[ExcludeFromCodeCoverage]
	public class EditModeController : Controller
	{
		private readonly ILogger<EditModeController> _logger;
		private readonly IEditModeService _editModeService;

		public EditModeController(ILogger<EditModeController> logger, IEditModeService editModeService)
		{
			_logger = logger;
			_editModeService = editModeService;
		}
		
		[Route("/api/admin/edit/saveChanges")]
		[HttpPost]
		[Authorize(Roles = "Administrator")]
		public IActionResult SaveChanges([FromBody]ContentChangeApiModel model)
		{
			if (ModelState.IsValid && Settings.AllowEditMode)
			{
				_editModeService.TrySaveChanges(model.Changes);
				CacheManager.Clear();
				return Ok();
			}

			return BadRequest();
		}

		
	}
}
