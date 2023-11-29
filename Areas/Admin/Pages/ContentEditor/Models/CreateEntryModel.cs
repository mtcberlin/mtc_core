using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Areas.Admin.Pages.ContentEditor.Models
{
	public class CreateEntryModel
	{
		[JsonProperty("path")]
		public string Path { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }
	}
}
