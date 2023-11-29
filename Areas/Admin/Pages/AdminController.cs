using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Areas.Admin.Pages
{
	public class AdminController : Controller
	{
		private readonly ILogger<AdminController> _logger;

		public AdminController(ILogger<AdminController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
			return Redirect("/admin/login");
		}

		

	}
}