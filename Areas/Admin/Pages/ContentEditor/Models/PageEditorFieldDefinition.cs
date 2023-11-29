
// ReSharper disable once CheckNamespace
using MtcMvcCore.Core.Models;

namespace MtcMvcCore.Areas.Admin.Pages.ContentEditor.Models
{
	public class PageEditorFieldDefinition
	{
		public string DisplayName { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public EditorConfigAttribute EditorConfigAttribute { get; set; }
	}
}
