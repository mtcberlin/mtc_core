using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Areas.Admin.Pages.ContentEditor.Models
{
	public class MoveEntryModel
	{
		[JsonProperty("oldpath")]
		public string OldPath { get; set; }

		[JsonProperty("newpath")]
		public string NewPath { get; set; }

	}
}
