using Microsoft.Extensions.Logging;
using Moq;
using MtcMvcCore.Core.Controllers;
using MtcMvcCore.Core.Services;
using Xunit;

namespace MtcMvcCore.Core.UnitTests
{
	public class EditModeServiceTests
	{
		private readonly IEditModeService _editModeService;

		public EditModeServiceTests()
		{
			var logger = new Mock<ILogger<EditModeService>>();
			_editModeService = new EditModeService(logger.Object);
			
		}

	}
}
