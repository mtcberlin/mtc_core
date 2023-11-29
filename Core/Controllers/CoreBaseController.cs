using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;

namespace MtcMvcCore.Core.Controllers
{
	[ExcludeFromCodeCoverage]
	// [BasicAuthorization]
	public class CoreBaseController : Microsoft.AspNetCore.Mvc.Controller {
		
		protected ActionResult CreateJsonResponse(bool success)
		{
			return CreateJsonResponse(success, new {});
		}
		protected ActionResult CreateJsonResponse<T>(bool success, T data, string message = "") where T : class
		{
			if (success)
			{

				return Json(new JsonResponse<T>
				{
					Success = success,
					Data = data
				});

			} else {

				return BadRequest(new JsonResponse<T>
				{
					Success = success,
					Data = data,
					Message = message
				});
				
			}
		}

		public class JsonResponse<T> where T : class
		{

			public bool Success { get; set; }
			public T Data { get; set; }
			public string Message { get; set; }

		}
		
	}
}
