using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Areas.Admin.Pages.ContentEditor.Models
{
	public class SaveFileContentModel
	{
		[JsonProperty("filepath")]
		public string Filepath { get; set; }

		[JsonProperty("content")]
		public string Content { get; set; }
	}
}
