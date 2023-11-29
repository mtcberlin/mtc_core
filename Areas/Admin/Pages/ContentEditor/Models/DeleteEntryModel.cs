using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Areas.Admin.Pages.ContentEditor.Models
{
	public class DeleteEntryModel
	{
		[JsonProperty("path")]
		public string Path { get; set; }
	}
}
