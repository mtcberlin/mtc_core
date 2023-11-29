using System;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Areas.Admin.Pages.ContentEditor.Models
{
	public class ChangeComponentModel
	{
		public Guid PageId { get; set; }

		public string Lang { get; set; }

		public string PlaceholderName { get; set; }
		
		public string ComponentName { get; set; }
		
		public string ComponentId { get; set; }

		public string NewPosition { get; set; }
	}
}
